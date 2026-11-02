using FluentAssertions;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.AnimalModule.Runtime.Scripts.Movement.Jump;
using Modules.AnimalModule.Runtime.Scripts.Movement.Linear;
using Modules.SharedModule.Runtime.Scripts.Tools;
using NUnit.Framework;
using UnityEngine;

namespace Modules.AnimalModule.Tests
{
    public class MovementControllersTests
    {
        private const float Precision = 0.001f;

        private GameObject _animal;
        private GameObject _ground;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private MovementModel _movementModel;

        [SetUp]
        public void SetUp()
        {
            _animal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _animal.transform.position = new Vector3(0, 0.5f, 0);
            _rigidbody = _animal.AddComponent<Rigidbody>();
            _collider = _animal.GetComponent<Collider>();

            _ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _ground.transform.position = new Vector3(0, -0.5f, 0);
            _ground.transform.localScale = new Vector3(10, 1, 10);

            Physics.SyncTransforms();
            _movementModel = new MovementModel(Vector3.right);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_animal);
            Object.DestroyImmediate(_ground);
        }

        [Test]
        public void LinearMovement_ReachesSpeedAndKeepsVertical()
        {
            var config = new LinearMovementConfig();
            var movement = new LinearMovementController(config, _movementModel, _rigidbody);
            _rigidbody.linearVelocity = new Vector3(0, -3, 0);

            movement.FixedTick(config.Speed / config.Acceleration);

            _rigidbody.linearVelocity.x.Should().BeApproximately(config.Speed, Precision);
            _rigidbody.linearVelocity.y.Should().BeApproximately(-3, Precision);
            _rigidbody.linearVelocity.z.Should().BeApproximately(0, Precision);
        }

        [Test]
        public void LinearMovement_AfterHit_RecoversWithLimitedAcceleration()
        {
            var config = new LinearMovementConfig();
            var movement = new LinearMovementController(config, _movementModel, _rigidbody);
            _rigidbody.linearVelocity = new Vector3(-5, 0, 0);

            movement.FixedTick(0.1f);

            _rigidbody.linearVelocity.x.Should().BeApproximately(-5 + config.Acceleration * 0.1f, Precision,
                "the push from the hit fades out instead of being cancelled at once");
        }

        [Test]
        public void LinearMovement_TurnsTowardsNewDirectionWithLimitedTurnSpeed()
        {
            var config = new LinearMovementConfig();
            var movement = new LinearMovementController(config, _movementModel, _rigidbody);
            var deltaTime = 30 / config.TurnSpeedInDegrees;
            _movementModel.SetDirection(Vector3.forward);

            movement.FixedTick(deltaTime);

            var horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            Vector3.Angle(Vector3.right, horizontalVelocity).Should().BeApproximately(30, 0.1f,
                "the heading turns by the turn speed only, not to the new direction at once");
        }

        [Test]
        public void LinearMovement_ReachesNewDirectionOverTime()
        {
            var config = new LinearMovementConfig();
            var movement = new LinearMovementController(config, _movementModel, _rigidbody);
            _movementModel.SetDirection(Vector3.left);

            for (var tick = 0; tick < 100; tick++)
                movement.FixedTick(0.02f);

            _rigidbody.linearVelocity.x.Should().BeApproximately(-config.Speed, Precision);
            _rigidbody.linearVelocity.y.Should().BeApproximately(0, Precision, "a turn back stays on the ground");
        }

        [Test]
        public void JumpMovement_BeforeInterval_DoesNotJump()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);

            movement.FixedTick(config.Interval * 0.9f);

            _rigidbody.linearVelocity.Should().Be(Vector3.zero);
        }

        [Test]
        public void JumpMovement_AfterIntervalOnTheGround_LaunchesForConfiguredDistance()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);

            movement.FixedTick(config.Interval);

            var expected = BallisticTools.GetLaunchVelocity(Vector3.right, config.Distance, config.AngleInDegrees,
                Physics.gravity.magnitude);
            (_rigidbody.linearVelocity - expected).magnitude.Should().BeLessThan(Precision);
        }

        [Test]
        public void JumpMovement_JumpsWhereTheBodyHasTurnedTo()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);
            var turnTime = 30 / config.TurnSpeedInDegrees;
            movement.FixedTick(config.Interval - turnTime + 0.001f);
            _movementModel.SetDirection(Vector3.forward);

            movement.FixedTick(turnTime);

            var horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            Vector3.Angle(Vector3.right, horizontalVelocity).Should().BeApproximately(30, 0.1f);
        }

        [Test]
        public void JumpMovement_InTheAir_WaitsForLanding()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);
            MoveAnimalTo(new Vector3(0, 3, 0));

            movement.FixedTick(config.Interval);
            _rigidbody.linearVelocity.Should().Be(Vector3.zero);

            MoveAnimalTo(new Vector3(0, 0.5f, 0));
            movement.FixedTick(0.02f);
            _rigidbody.linearVelocity.y.Should().BePositive("the jump that waited happens right after landing");
        }

        [Test]
        public void JumpMovement_Landing_StopsHorizontalSliding()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);
            MoveAnimalTo(new Vector3(0, 3, 0));
            movement.FixedTick(0.02f);

            MoveAnimalTo(new Vector3(0, 0.5f, 0));
            _rigidbody.linearVelocity = new Vector3(2, -1, 1);
            movement.FixedTick(0.02f);

            _rigidbody.linearVelocity.Should().Be(new Vector3(0, -1, 0));
        }

        [Test]
        public void JumpMovement_HitOnTheGround_KeepsPush()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);
            _rigidbody.linearVelocity = new Vector3(2, 0, 0);

            movement.FixedTick(0.02f);

            _rigidbody.linearVelocity.x.Should().Be(2, "only a landing stops the sliding, not standing on the ground");
        }

        [Test]
        public void JumpMovement_OnAnotherAnimal_DoesNotJump()
        {
            var config = new JumpMovementConfig();
            var movement = CreateJumpMovement(config);
            var otherAnimal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            otherAnimal.AddComponent<Rigidbody>();
            otherAnimal.transform.position = new Vector3(0, 0.5f, 0);
            MoveAnimalTo(new Vector3(0, 1.5f, 0));

            movement.FixedTick(config.Interval);

            _rigidbody.linearVelocity.Should().Be(Vector3.zero);
            Object.DestroyImmediate(otherAnimal);
        }

        private JumpMovementController CreateJumpMovement(JumpMovementConfig config) =>
            new JumpMovementController(config, _movementModel, _rigidbody, _collider);

        private void MoveAnimalTo(Vector3 position)
        {
            _animal.transform.position = position;
            Physics.SyncTransforms();
        }
    }
}
