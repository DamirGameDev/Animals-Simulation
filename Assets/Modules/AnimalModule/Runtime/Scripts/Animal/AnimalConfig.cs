using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    /// <summary>
    /// Everything that makes one kind of animal different from another. A new animal is a new asset of this type.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(AnimalConfig), menuName = "Configs/Animals/" + nameof(AnimalConfig))]
    public class AnimalConfig : SerializedScriptableObject
    {
        [Tooltip("Prefab with AnimalSerializableComponents")]
        [field: SerializeField] public AssetReferenceGameObject Prefab { get; private set; }
        [field: SerializeField] public FoodChainConfig FoodChain { get; private set; } = new FoodChainConfig();

        public IReadOnlyList<IAnimalBehaviourConfig> Behaviours => _behaviours;

        [Tooltip("What the animal does every physics tick, in this order")]
        [OdinSerialize] private List<IAnimalBehaviourConfig> _behaviours = new List<IAnimalBehaviourConfig>();
    }
}
