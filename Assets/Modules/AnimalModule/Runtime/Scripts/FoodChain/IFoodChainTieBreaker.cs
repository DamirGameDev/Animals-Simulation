namespace Modules.AnimalModule.Runtime.Scripts.FoodChain
{
    /// <summary>
    /// Decides who survives when two animals could eat each other.
    /// </summary>
    public interface IFoodChainTieBreaker
    {
        bool IsFirstWinner(FoodChainConfig first, FoodChainConfig second);
    }
}
