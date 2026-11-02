using Modules.AnimalModule.Runtime.Scripts.FoodChain;

namespace Modules.AnimalModule.Tests.Fakes
{
    public class FakeFoodChainTieBreaker : IFoodChainTieBreaker
    {
        public bool IsFirstWinner { get; set; } = true;
        public int CallsCount { get; private set; }

        bool IFoodChainTieBreaker.IsFirstWinner(FoodChainConfig first, FoodChainConfig second)
        {
            CallsCount++;
            return IsFirstWinner;
        }
    }
}
