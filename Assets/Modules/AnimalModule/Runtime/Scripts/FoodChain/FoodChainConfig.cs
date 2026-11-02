using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.FoodChain
{
    /// <summary>
    /// The animal's place in the food chain: what kind of food it is and which kinds it eats.
    /// </summary>
    [Serializable]
    public class FoodChainConfig
    {
        [field: SerializeField] public FoodChainTag Tag { get; private set; }

        /// <summary>
        /// How the animal is counted in the statistics. Only the diet decides who eats whom.
        /// </summary>
        [field: SerializeField] public FoodChainRole Role { get; private set; }

        public IReadOnlyList<FoodChainTag> Eats => _eats;

        [Tooltip("Tags of the animals this one eats")]
        [SerializeField] private List<FoodChainTag> _eats = new List<FoodChainTag>();

        public FoodChainConfig()
        {
        }

        public FoodChainConfig(FoodChainTag tag, FoodChainRole role, params FoodChainTag[] eats)
        {
            Tag = tag;
            Role = role;
            _eats = new List<FoodChainTag>(eats);
        }
    }
}
