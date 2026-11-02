using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement
{
    /// <summary>
    /// Movement state of one animal. Movement controllers read the direction,
    /// other behaviours (returning to the screen, collisions) may change it.
    /// </summary>
    public class MovementModel
    {
        private const float MinSqrMagnitude = 0.0001f;

        public Vector3 Direction { get; private set; }

        public MovementModel(Vector3 direction)
        {
            SetDirection(direction);
        }

        /// <summary>
        /// Keeps the direction on the ground plane and normalized. A zero vector has no direction, so the previous one is kept.
        /// </summary>
        public void SetDirection(Vector3 direction)
        {
            direction.y = 0;

            if (direction.sqrMagnitude < MinSqrMagnitude)
                return;

            Direction = direction.normalized;
        }
    }
}
