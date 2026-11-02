using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.SharedModule.Runtime.Scripts.Tools;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Linear
{
    /// <summary>
    /// Moves with a constant speed along its heading, which turns towards the wanted direction with a limited
    /// turn speed, so the animal moves along an arc instead of turning on the spot. The speed changes with
    /// limited acceleration, so a hit from another body still pushes the animal for a moment.
    /// Vertical velocity is left to gravity.
    /// </summary>
    public class LinearMovementController : IAnimalBehaviour
    {
        private readonly LinearMovementConfig _config;
        private readonly MovementModel _movementModel;
        private readonly Rigidbody _rigidbody;

        private Vector3 _heading;

        public LinearMovementController(LinearMovementConfig config, MovementModel movementModel, Rigidbody rigidbody)
        {
            _config = config;
            _movementModel = movementModel;
            _rigidbody = rigidbody;
            _heading = movementModel.Direction;
        }

        public void FixedTick(float deltaTime)
        {
            _heading = DirectionTools.TurnTowardsOnGround(_heading, _movementModel.Direction,
                _config.TurnSpeedInDegrees * deltaTime);

            var velocity = _rigidbody.linearVelocity;
            var horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
            var targetVelocity = _heading * _config.Speed;

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, _config.Acceleration * deltaTime);

            _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
            _rigidbody.MoveRotation(Quaternion.LookRotation(_heading));
        }
    }
}
