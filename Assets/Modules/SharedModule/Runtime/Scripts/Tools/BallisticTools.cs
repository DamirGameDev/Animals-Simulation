using UnityEngine;

namespace Modules.SharedModule.Runtime.Scripts.Tools
{
    public static class BallisticTools
    {
        /// <summary>
        /// Velocity that lands a body <paramref name="distance"/> away on flat ground,
        /// from the range formula distance = speed² · sin(2·angle) / gravity.
        /// </summary>
        public static Vector3 GetLaunchVelocity(Vector3 direction, float distance, float angleInDegrees, float gravity)
        {
            var angle = angleInDegrees * Mathf.Deg2Rad;
            var speed = Mathf.Sqrt(distance * gravity / Mathf.Sin(2 * angle));
            var horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

            return horizontalDirection * (speed * Mathf.Cos(angle)) + Vector3.up * (speed * Mathf.Sin(angle));
        }
    }
}
