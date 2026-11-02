using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.SharedModule.Runtime.Scripts.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Modules.AnimalModule.Runtime.Scripts.Spawn
{
    [ConfigScope(nameof(AnimalsSpawnConfig))]
    [CreateAssetMenu(fileName = nameof(AnimalsSpawnConfig), menuName = "Configs/Animals/" + nameof(AnimalsSpawnConfig))]
    public class AnimalsSpawnConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float MinSpawnInterval { get; private set; } = 1;
        [field: SerializeField, Min(0)] public float MaxSpawnInterval { get; private set; } = 2;

        [field: Tooltip("Spawning pauses while this many animals are alive")]
        [field: SerializeField, Min(1)] public int MaxAnimalsCount { get; private set; } = 30;

        [field: Tooltip("Distance from the screen edges in viewport units, so animals don't appear half visible")]
        [field: SerializeField, Range(0, 0.5f)] public float ViewportMargin { get; private set; } = 0.1f;

        [field: SerializeField, Min(0)] public float SpawnHeight { get; private set; } = 0.5f;

        [field: Tooltip("A new animal looks for a spot with no other animal within this radius")]
        [field: SerializeField, Min(0)] public float SpawnClearanceRadius { get; private set; } = 1.2f;

        public IReadOnlyList<AssetReferenceT<AnimalConfig>> Animals => _animals;

        [Tooltip("References, not direct links: each kind of animal is loaded when it spawns for the first time")]
        [SerializeField] private List<AssetReferenceT<AnimalConfig>> _animals = new List<AssetReferenceT<AnimalConfig>>();

        private void OnValidate()
        {
            if (!IsValid(out var error))
                Debug.LogWarning($"{name}: {error}", this);
        }

        public bool IsValid(out string error)
        {
            if (_animals.Count == 0 || _animals.Exists(animal => animal == null || !animal.RuntimeKeyIsValid()))
                error = "the animals list is empty or has an empty slot";
            else if (MinSpawnInterval > MaxSpawnInterval)
                error = "MinSpawnInterval is greater than MaxSpawnInterval";
            else
                error = null;

            return error == null;
        }
    }
}
