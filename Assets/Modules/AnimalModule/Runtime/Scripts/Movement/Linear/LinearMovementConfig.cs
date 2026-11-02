using System;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Linear
{
    [Serializable]
    public class LinearMovementConfig : IAnimalBehaviourConfig
    {
        [field: SerializeField, Min(0)] public float Speed { get; private set; } = 2;

        [field: Tooltip("How fast the speed comes back to Speed after a hit, in units per second squared")]
        [field: SerializeField, Min(0.01f)] public float Acceleration { get; private set; } = 8;

        [field: Tooltip("How fast the heading turns towards the wanted direction, in degrees per second")]
        [field: SerializeField, Min(1)] public float TurnSpeedInDegrees { get; private set; } = 150;
    }
}
