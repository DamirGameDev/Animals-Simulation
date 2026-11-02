using System.Linq;

namespace Modules.AnimalModule.Runtime.Scripts.FoodChain
{
    /// <summary>
    /// Tells which animal of a pair eats the other one: an animal eats another if the other's tag is in its diet.
    /// When both could eat each other, the tie breaker decides.
    /// </summary>
    public class FoodChainEaterProvider
    {
        private readonly IFoodChainTieBreaker _tieBreaker;

        public FoodChainEaterProvider(IFoodChainTieBreaker tieBreaker)
        {
            _tieBreaker = tieBreaker;
        }

        public FoodChainEater GetEater(FoodChainConfig first, FoodChainConfig second)
        {
            var firstCanEat = CanEat(first, second);
            var secondCanEat = CanEat(second, first);

            if (firstCanEat && secondCanEat)
                return _tieBreaker.IsFirstWinner(first, second) ? FoodChainEater.First : FoodChainEater.Second;

            if (firstCanEat)
                return FoodChainEater.First;

            return secondCanEat ? FoodChainEater.Second : FoodChainEater.None;
        }

        private static bool CanEat(FoodChainConfig eater, FoodChainConfig food) =>
            food.Tag != null && eater.Eats.Contains(food.Tag);
    }
}
