using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Collisions;

namespace Modules.AnimalModule.Runtime.Scripts.FoodChain
{
    /// <summary>
    /// The eater kills the other animal and resolves the collision. Prey bumping into prey is left to physics
    /// and to the next rules.
    /// </summary>
    public class FoodChainCollisionRule : ICollisionRule
    {
        private readonly FoodChainEaterProvider _eaterProvider;

        public FoodChainCollisionRule(FoodChainEaterProvider eaterProvider)
        {
            _eaterProvider = eaterProvider;
        }

        public CollisionRuleResult Apply(AnimalModel first, AnimalModel second)
        {
            switch (_eaterProvider.GetEater(first.Config.FoodChain, second.Config.FoodChain))
            {
                case FoodChainEater.First:
                    second.Die(first);
                    return CollisionRuleResult.Stop;
                case FoodChainEater.Second:
                    first.Die(second);
                    return CollisionRuleResult.Stop;
                default:
                    return CollisionRuleResult.Continue;
            }
        }
    }
}
