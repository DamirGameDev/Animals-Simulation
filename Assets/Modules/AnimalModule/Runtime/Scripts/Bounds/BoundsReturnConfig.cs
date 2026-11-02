using System;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Bounds
{
    [Serializable]
    public class BoundsReturnConfig : IAnimalBehaviourConfig
    {
        [field: Tooltip("The animal turns back this far from the screen edges, in viewport units, " +
                        "so it doesn't leave the screen before turning")]
        [field: SerializeField, Range(0, 0.4f)] public float ViewportMargin { get; private set; } = 0.08f;

        [field: Tooltip("Random turn away from the exact direction to the center, so returning animals don't line up")]
        [field: SerializeField, Range(0, 90)] public float MaxDeviationInDegrees { get; private set; } = 30;
    }
}
