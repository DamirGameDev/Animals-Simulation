using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.FoodChain
{
    /// <summary>
    /// A kind of food in the food chain ("Frog", "Snake", "Fly"...). An asset, so the set of kinds stays open.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(FoodChainTag), menuName = "Configs/Animals/" + nameof(FoodChainTag))]
    public class FoodChainTag : ScriptableObject
    {
    }
}
