using System;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Wander
{
    [Serializable]
    public class WanderConfig : IAnimalBehaviourConfig
    {
        [field: SerializeField, Min(0.01f)] public float Interval { get; private set; } = 2;

        [field: Tooltip("The direction turns by a random angle up to this one, to either side")]
        [field: SerializeField, Range(0, 180)] public float MaxTurnInDegrees { get; private set; } = 45;
    }
}
