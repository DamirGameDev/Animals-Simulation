using Modules.SharedModule.Runtime.Scripts.Services;
using UnityEngine;

namespace Modules.SharedModule.Runtime.Scripts.Tools
{
    public static class RandomServiceTools
    {
        public static Vector3 GetDirectionOnGround(this IRandomService randomService) =>
            Quaternion.Euler(0, randomService.Range(0f, 360f), 0) * Vector3.forward;
    }
}
