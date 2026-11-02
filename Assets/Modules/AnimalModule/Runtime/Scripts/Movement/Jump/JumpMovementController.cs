using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.SharedModule.Runtime.Scripts.Tools;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Jump
{
    /// <summary>
    /// Every Interval seconds launches the body so that it lands Distance away along the current direction.
    /// Jumps only from the ground: if the interval passes in the air, the jump waits for the landing.
    /// The landing stops the horizontal sliding, so the jump ends at Distance; a hit on the ground still pushes it.
    /// On the ground the body turns smoothly towards the wanted direction and jumps where it faces.
    /// </summary>
    public class JumpMovementController : IAnimalBehaviour
    {
        private const float GroundCheckMargin = 0.05f;

        private readonly JumpMovementConfig _config;
        private readonly MovementModel _movementModel;
        private readonly Rigidbody _rigidbody;
        private readonly Collider _collider;
        private readonly RaycastHit[] _groundHits = new RaycastHit[4];

        private float _timeSinceJump;
        private bool _wasGrounded = true;
        private Vector3 _heading;

        public JumpMovementController(JumpMovementConfig config, MovementModel movementModel, Rigidbody rigidbody,
            Collider collider)
        {
            _config = config;
            _movementModel = movementModel;
            _rigidbody = rigidbody;
            _collider = collider;
            _heading = movementModel.Direction;
        }

        public void FixedTick(float deltaTime)
        {
            _timeSinceJump += deltaTime;

            var isGrounded = IsGrounded();
            var hasLanded = isGrounded && !_wasGrounded;
            _wasGrounded = isGrounded;

            if (hasLanded)
                StopSliding();

            if (!isGrounded)
                return;

            TurnTowardsDirection(deltaTime);

            if (_timeSinceJump < _config.Interval)
                return;

            _timeSinceJump = 0;
            _rigidbody.linearVelocity = BallisticTools.GetLaunchVelocity(_heading, _config.Distance,
                _config.AngleInDegrees, Physics.gravity.magnitude);
        }

        private void TurnTowardsDirection(float deltaTime)
        {
            _heading = DirectionTools.TurnTowardsOnGround(_heading, _movementModel.Direction,
                _config.TurnSpeedInDegrees * deltaTime);
            _rigidbody.MoveRotation(Quaternion.LookRotation(_heading));
        }

        private void StopSliding()
        {
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        }

        /// <summary>
        /// A static collider right under the body. The top of a jump or standing on another animal doesn't count.
        /// </summary>
        private bool IsGrounded()
        {
            var bounds = _collider.bounds;
            var count = Physics.RaycastNonAlloc(bounds.center, Vector3.down, _groundHits,
                bounds.extents.y + GroundCheckMargin, Physics.AllLayers, QueryTriggerInteraction.Ignore);

            for (var index = 0; index < count; index++)
            {
                var hit = _groundHits[index];

                if (hit.collider != _collider && hit.rigidbody == null)
                    return true;
            }

            return false;
        }
    }
}
