using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.AnimalModule.Runtime.Scripts.Movement.Wander;
using Modules.AnimalModule.Tests.Fakes;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class WanderBehaviourTests
    {
        private readonly WanderConfig _config = new WanderConfig();

        private MovementModel _movementModel;
        private FakeRandomService _randomService;
        private WanderBehaviour _behaviour;

        [SetUp]
        public void SetUp()
        {
            _movementModel = new MovementModel(Vector3.forward);
            _randomService = new FakeRandomService { FloatRangePosition = 1 };
            _behaviour = new WanderBehaviour(_config, _movementModel, _randomService);
        }

        [Test]
        public void BeforeInterval_KeepsDirection()
        {
            _behaviour.FixedTick(_config.Interval * 0.9f);

            _movementModel.Direction.Should().Be(Vector3.forward);
        }

        [TestCase(0f, -1)]
        [TestCase(1f, 1)]
        public void AfterInterval_TurnsByRandomUpToMax(float randomRangePosition, int side)
        {
            _randomService.FloatRangePosition = randomRangePosition;

            _behaviour.FixedTick(_config.Interval);

            Vector3.SignedAngle(Vector3.forward, _movementModel.Direction, Vector3.up)
                .Should().BeApproximately(side * _config.MaxTurnInDegrees, 0.01f);
        }

        [Test]
        public void IntervalRestartsAfterTurn()
        {
            _behaviour.FixedTick(_config.Interval);
            var directionAfterTurn = _movementModel.Direction;

            _behaviour.FixedTick(_config.Interval * 0.9f);

            _movementModel.Direction.Should().Be(directionAfterTurn);
        }
    }
}
