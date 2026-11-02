using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Bounds;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.AnimalModule.Runtime.Scripts.Spawn;
using Modules.AnimalModule.Tests.Fakes;
using Modules.SharedModule.Runtime.Scripts.Configs;
using Modules.SharedModule.Runtime.Scripts.Services;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using VContainer;

namespace Modules.AnimalModule.Tests
{
    /// <summary>
    /// Checks the real config assets of the project against the real registrations, so a broken animal is found
    /// here and not on its first spawn.
    /// </summary>
    public class AnimalConfigsValidationTests
    {
        private static IEnumerable<TestCaseData> AnimalConfigs =>
            FindAssets<AnimalConfig>().Select(config => new TestCaseData(config));

        [TestCaseSource(nameof(AnimalConfigs))]
        public void AnimalConfig_HasAddressablePrefabWithComponents(AnimalConfig config)
        {
            var prefab = config.Prefab.editorAsset;

            prefab.Should().NotBeNull("the prefab reference must point to an asset");
            IsAddressable(config.Prefab.AssetGUID).Should().BeTrue("the prefab is loaded through Addressables");

            var serializableComponents = prefab.GetComponent<AnimalSerializableComponents>();
            serializableComponents.Should().NotBeNull();
            serializableComponents.Rigidbody.Should().NotBeNull();
            serializableComponents.Collider.Should().NotBeNull();
            serializableComponents.CollisionObserver.Should().NotBeNull();
        }

        [TestCaseSource(nameof(AnimalConfigs))]
        public void AnimalConfig_HasFoodChainTag(AnimalConfig config)
        {
            config.FoodChain.Tag.Should().NotBeNull();
            config.FoodChain.Eats.Should().NotContainNulls();
        }

        [TestCaseSource(nameof(AnimalConfigs))]
        public void AnimalConfig_BehavioursAreBuiltByRegisteredFactories(AnimalConfig config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance<IScreenBounds>(new FakeScreenBounds(10));
            builder.RegisterInstance<IRandomService>(new FakeRandomService());
            AnimalModuleDependencies.RegisterBehaviourFactories(builder);

            using var container = builder.Build();
            var behavioursFactory = container.Resolve<AnimalBehavioursFactory>();
            var model = new AnimalModel(config, new MovementModel(Vector3.forward), new HealthModel());
            var serializableComponents = config.Prefab.editorAsset.GetComponent<AnimalSerializableComponents>();

            config.Behaviours.Should().NotBeEmpty();
            behavioursFactory.Invoking(factory => factory.Create(config.Behaviours, model, serializableComponents))
                .Should().NotThrow("every behaviour config needs a factory registered in AnimalModuleDependencies");
        }

        [Test]
        public void AnimalsSpawnConfig_IsValidAndPointsToAddressableAnimalConfigs()
        {
            var spawnConfigs = FindAssets<AnimalsSpawnConfig>();

            spawnConfigs.Should().ContainSingle();

            var spawnConfig = spawnConfigs[0];
            var scope = typeof(AnimalsSpawnConfig).GetCustomAttribute<ConfigScopeAttribute>().Scope;
            var spawnConfigGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(spawnConfig));

            spawnConfig.IsValid(out var error).Should().BeTrue(error);
            GetAddress(spawnConfigGuid).Should().Be(scope, "the config is loaded by its scope key");

            foreach (var animal in spawnConfig.Animals)
            {
                animal.editorAsset.Should().NotBeNull();
                IsAddressable(animal.AssetGUID).Should().BeTrue($"{animal.editorAsset.name} is loaded through Addressables");
            }
        }

        private static List<T> FindAssets<T>() where T : ScriptableObject =>
            AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

        private static bool IsAddressable(string guid) => GetAddress(guid) != null;

        private static string GetAddress(string guid) =>
            AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid)?.address;
    }
}
