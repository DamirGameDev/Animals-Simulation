using Modules.SharedModule.Runtime.Scripts.Services;

namespace Modules.AnimalModule.Runtime.Scripts.FoodChain
{
    public class RandomFoodChainTieBreaker : IFoodChainTieBreaker
    {
        private readonly IRandomService _randomService;

        public RandomFoodChainTieBreaker(IRandomService randomService)
        {
            _randomService = randomService;
        }

        public bool IsFirstWinner(FoodChainConfig first, FoodChainConfig second) => _randomService.Range(0, 2) == 0;
    }
}
