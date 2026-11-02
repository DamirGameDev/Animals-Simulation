using System.Reflection;
using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Death;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.HudModule.Runtime.Scripts.DeathCounters;
using NUnit.Framework;
using UnityEngine;

namespace Modules.HudModule.Tests
{
    public class DeathCountersMediatorTests
    {
        private AnimalsRepository _repository;
        private AnimalDeathsObserver _deathsObserver;
        private DeathCountersModel _model;
        private DeathCountersMediator _mediator;
        private AnimalConfig _frogConfig;
        private AnimalConfig _snakeConfig;
        private FoodChainTag _frogTag;
        private FoodChainTag _snakeTag;
        private GameObject _views;

        [SetUp]
        public void SetUp()
        {
            _views = new GameObject("Views");
            _repository = new AnimalsRepository();
            _deathsObserver = new AnimalDeathsObserver(_repository);
            _model = new DeathCountersModel();
            _mediator = new DeathCountersMediator(_deathsObserver, _model);
            _deathsObserver.Initialize();
            _mediator.Initialize();

            _frogTag = ScriptableObject.CreateInstance<FoodChainTag>();
            _snakeTag = ScriptableObject.CreateInstance<FoodChainTag>();
            _frogConfig = CreateConfig(new FoodChainConfig(_frogTag, FoodChainRole.Prey));
            _snakeConfig = CreateConfig(new FoodChainConfig(_snakeTag, FoodChainRole.Predator, _frogTag, _snakeTag));
        }

        [TearDown]
        public void TearDown()
        {
            _mediator.Dispose();
            _deathsObserver.Dispose();
            Object.DestroyImmediate(_views);
            Object.DestroyImmediate(_frogConfig);
            Object.DestroyImmediate(_snakeConfig);
            Object.DestroyImmediate(_frogTag);
            Object.DestroyImmediate(_snakeTag);
        }

        [Test]
        public void DeathsAreCountedByVictimRole()
        {
            var snake = AddAnimal(_snakeConfig);
            var frog = AddAnimal(_frogConfig);
            var otherSnake = AddAnimal(_snakeConfig);

            frog.Model.Die(snake.Model);
            otherSnake.Model.Die(snake.Model);

            _model.GetCount(FoodChainRole.Prey).Should().Be(1);
            _model.GetCount(FoodChainRole.Predator).Should().Be(1);
        }

        [Test]
        public void AfterDispose_DeathsAreNotCounted()
        {
            var frog = AddAnimal(_frogConfig);
            _mediator.Dispose();

            frog.Model.Die(null);

            _model.GetCount(FoodChainRole.Prey).Should().Be(0);
        }

        private AnimalController AddAnimal(AnimalConfig config)
        {
            var serializableComponents = new GameObject("Animal").AddComponent<AnimalSerializableComponents>();
            serializableComponents.transform.SetParent(_views.transform);

            var model = new AnimalModel(config, new MovementModel(Vector3.forward), new HealthModel());
            var animal = new AnimalController(model, serializableComponents, new IAnimalBehaviour[0]);
            _repository.Add(model, animal);

            return animal;
        }

        private static AnimalConfig CreateConfig(FoodChainConfig foodChain)
        {
            var config = ScriptableObject.CreateInstance<AnimalConfig>();
            typeof(AnimalConfig)
                .GetField("<FoodChain>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(config, foodChain);

            return config;
        }
    }
}
