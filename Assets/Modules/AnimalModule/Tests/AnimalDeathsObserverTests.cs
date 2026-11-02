using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Death;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class AnimalDeathsObserverTests
    {
        private AnimalsRepository _repository;
        private AnimalDeathsObserver _observer;
        private GameObject _views;

        private AnimalController _reportedVictim;
        private AnimalController _reportedKiller;
        private int _reportsCount;

        [SetUp]
        public void SetUp()
        {
            _views = new GameObject("Views");
            _repository = new AnimalsRepository();
            _observer = new AnimalDeathsObserver(_repository);
            _observer.Initialize();
            _observer.Died += (victim, killer) =>
            {
                _reportsCount++;
                _reportedVictim = victim;
                _reportedKiller = killer;
            };
        }

        [TearDown]
        public void TearDown()
        {
            _observer.Dispose();
            Object.DestroyImmediate(_views);
            _reportsCount = 0;
            _reportedVictim = null;
            _reportedKiller = null;
        }

        [Test]
        public void AnimalInRepositoryIsEaten_ReportsVictimAndKiller()
        {
            var snake = AddAnimal();
            var frog = AddAnimal();

            frog.Model.Die(snake.Model);

            _reportsCount.Should().Be(1);
            _reportedVictim.Should().BeSameAs(frog);
            _reportedKiller.Should().BeSameAs(snake);
        }

        [Test]
        public void AnimalDiesWithoutKiller_ReportsNullKiller()
        {
            var frog = AddAnimal();

            frog.Model.Die(null);

            _reportsCount.Should().Be(1);
            _reportedVictim.Should().BeSameAs(frog);
            _reportedKiller.Should().BeNull();
        }

        [Test]
        public void AnimalRemovedFromRepository_IsNoLongerObserved()
        {
            var frog = AddAnimal();
            _repository.RemoveByValue(frog);

            frog.Model.Die(null);

            _reportsCount.Should().Be(0);
        }

        private AnimalController AddAnimal()
        {
            var serializableComponents = new GameObject("Animal").AddComponent<AnimalSerializableComponents>();
            serializableComponents.transform.SetParent(_views.transform);

            var model = new AnimalModel(null, new MovementModel(Vector3.forward), new HealthModel());
            var animal = new AnimalController(model, serializableComponents, new IAnimalBehaviour[0]);
            _repository.Add(model, animal);

            return animal;
        }
    }
}
