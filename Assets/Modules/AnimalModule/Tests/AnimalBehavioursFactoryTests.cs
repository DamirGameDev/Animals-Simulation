using System;
using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class AnimalBehavioursFactoryTests
    {
        private readonly AnimalModel _model =
            new AnimalModel(null, new MovementModel(Vector3.forward), new HealthModel());

        [Test]
        public void Create_BuildsBehavioursByConfigTypeInConfigOrder()
        {
            var factory = new AnimalBehavioursFactory(new IAnimalBehaviourFactory[]
            {
                new FirstBehaviourFactory(), new SecondBehaviourFactory()
            });

            var behaviours = factory.Create(new IAnimalBehaviourConfig[] { new SecondConfig(), new FirstConfig() },
                _model, null);

            behaviours.Should().HaveCount(2);
            behaviours[0].Should().BeOfType<SecondBehaviour>();
            behaviours[1].Should().BeOfType<FirstBehaviour>();
        }

        [Test]
        public void Create_WithUnknownConfig_Throws()
        {
            var factory = new AnimalBehavioursFactory(new IAnimalBehaviourFactory[] { new FirstBehaviourFactory() });

            factory.Invoking(behavioursFactory =>
                    behavioursFactory.Create(new IAnimalBehaviourConfig[] { new SecondConfig() }, _model, null))
                .Should().Throw<InvalidOperationException>().WithMessage($"*{nameof(SecondConfig)}*");
        }

        [Test]
        public void Create_WithNullConfig_Throws()
        {
            var factory = new AnimalBehavioursFactory(new IAnimalBehaviourFactory[] { new FirstBehaviourFactory() });

            factory.Invoking(behavioursFactory =>
                    behavioursFactory.Create(new IAnimalBehaviourConfig[] { null }, _model, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Constructor_WithTwoFactoriesForOneConfig_Throws()
        {
            Action createFactory = () => new AnimalBehavioursFactory(new IAnimalBehaviourFactory[]
            {
                new FirstBehaviourFactory(), new FirstBehaviourFactory()
            });

            createFactory.Should().Throw<ArgumentException>();
        }

        private class FirstConfig : IAnimalBehaviourConfig
        {
        }

        private class SecondConfig : IAnimalBehaviourConfig
        {
        }

        private class FirstBehaviour : IAnimalBehaviour
        {
            public void FixedTick(float deltaTime)
            {
            }
        }

        private class SecondBehaviour : IAnimalBehaviour
        {
            public void FixedTick(float deltaTime)
            {
            }
        }

        private class FirstBehaviourFactory : AnimalBehaviourFactory<FirstConfig>
        {
            protected override IAnimalBehaviour Create(FirstConfig config, AnimalModel model,
                AnimalSerializableComponents serializableComponents) => new FirstBehaviour();
        }

        private class SecondBehaviourFactory : AnimalBehaviourFactory<SecondConfig>
        {
            protected override IAnimalBehaviour Create(SecondConfig config, AnimalModel model,
                AnimalSerializableComponents serializableComponents) => new SecondBehaviour();
        }
    }
}
