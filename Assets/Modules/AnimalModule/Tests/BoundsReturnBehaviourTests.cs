using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Bounds;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.AnimalModule.Tests.Fakes;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class BoundsReturnBehaviourTests
    {
        private readonly BoundsReturnConfig _config = new BoundsReturnConfig();

        private GameObject _animal;
        private MovementModel _movementModel;
        private FakeRandomService _randomService;
        private BoundsReturnBehaviour _behaviour;

        [SetUp]
        public void SetUp()
        {
            _animal = new GameObject("Animal");
            _movementModel = new MovementModel(Vector3.right);
            _randomService = new FakeRandomService();
            _behaviour = new BoundsReturnBehaviour(_config, _movementModel, _animal.transform,
                new FakeScreenBounds(10), _randomService);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_animal);
        }

        [Test]
        public void InsideBounds_KeepsDirection()
        {
            _animal.transform.position = new Vector3(5, 0, 0);

            _behaviour.FixedTick(0.02f);

            _movementModel.Direction.Should().Be(Vector3.right);
        }

        [Test]
        public void OnScreenButWithinMarginAndMovingAway_TurnsTowardsCenter()
        {
            _animal.transform.position = new Vector3(9.5f, 0, 0);

            _behaviour.FixedTick(0.02f);

            _movementModel.Direction.Should().Be(Vector3.left, "the animal turns back before it reaches the screen edge");
        }

        [Test]
        public void OutsideBoundsAndMovingAway_TurnsTowardsCenter()
        {
            _animal.transform.position = new Vector3(15, 0, 0);

            _behaviour.FixedTick(0.02f);

            _movementModel.Direction.Should().Be(Vector3.left);
        }

        [TestCase(0f, -1)]
        [TestCase(1f, 1)]
        public void OutsideBounds_DeviatesFromCenterDirectionByRandomUpToMax(float randomRangePosition, int side)
        {
            _randomService.FloatRangePosition = randomRangePosition;
            _animal.transform.position = new Vector3(15, 0, 0);

            _behaviour.FixedTick(0.02f);

            Vector3.SignedAngle(Vector3.left, _movementModel.Direction, Vector3.up)
                .Should().BeApproximately(side * _config.MaxDeviationInDegrees, 0.01f);
        }

        [Test]
        public void OutsideBoundsAndAlreadyReturning_KeepsDirection()
        {
            _animal.transform.position = new Vector3(-15, 0, 3);

            _behaviour.FixedTick(0.02f);

            _movementModel.Direction.Should().Be(Vector3.right);
        }
    }
}
