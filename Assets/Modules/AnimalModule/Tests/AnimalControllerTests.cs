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
    public class AnimalControllerTests
    {
        [Test]
        public void Dispose_DisposesDisposableBehaviours()
        {
            var disposableBehaviour = new DisposableBehaviour();
            var controller = new AnimalController(CreateModel(), null,
                new IAnimalBehaviour[] { new PlainBehaviour(), disposableBehaviour });

            controller.Dispose();

            disposableBehaviour.IsDisposed.Should().BeTrue();
        }

        [Test]
        public void RepositoryDispose_DisposesAnimalsLeftInIt()
        {
            var views = new GameObject("Views");
            var disposableBehaviour = new DisposableBehaviour();
            var repository = new AnimalsRepository();
            var model = CreateModel();
            repository.Add(model, new AnimalController(model, views.AddComponent<AnimalSerializableComponents>(),
                new IAnimalBehaviour[] { disposableBehaviour }));

            repository.Dispose();

            disposableBehaviour.IsDisposed.Should().BeTrue();
            UnityEngine.Object.DestroyImmediate(views);
        }

        private static AnimalModel CreateModel() =>
            new AnimalModel(null, new MovementModel(Vector3.forward), new HealthModel());

        private class PlainBehaviour : IAnimalBehaviour
        {
            public void FixedTick(float deltaTime)
            {
            }
        }

        private class DisposableBehaviour : IAnimalBehaviour, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void FixedTick(float deltaTime)
            {
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
