using System;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Jump
{
    [Serializable]
    public class JumpMovementConfig : IAnimalBehaviourConfig
    {
        [field: SerializeField, Min(0.1f)] public float Interval { get; private set; } = 1.5f;
        [field: SerializeField, Min(0)] public float Distance { get; private set; } = 3;
        [field: SerializeField, Range(10, 80)] public float AngleInDegrees { get; private set; } = 45;

        [field: Tooltip("How fast the body turns towards the wanted direction on the ground, in degrees per second")]
        [field: SerializeField, Min(1)] public float TurnSpeedInDegrees { get; private set; } = 180;
    }
}
