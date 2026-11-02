using UnityEngine;

namespace Modules.SharedModule.Runtime.Scripts.Tools
{
    public static class DirectionTools
    {
        /// <summary>
        /// Turns a direction on the ground towards the target by at most <paramref name="maxDegrees"/> around
        /// the vertical axis, so even a turn back stays on the ground.
        /// </summary>
        public static Vector3 TurnTowardsOnGround(Vector3 current, Vector3 target, float maxDegrees)
        {
            var angle = Vector3.SignedAngle(current, target, Vector3.up);
            var step = Mathf.Clamp(angle, -maxDegrees, maxDegrees);

            return Quaternion.Euler(0, step, 0) * current;
        }
    }
}
