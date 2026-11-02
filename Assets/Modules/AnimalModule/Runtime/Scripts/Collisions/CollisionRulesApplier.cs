using System;
using System.Collections.Generic;
using System.Linq;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts.Collisions
{
    /// <summary>
    /// Applies the collision rules in registration order until one of them stops the chain.
    /// </summary>
    public class CollisionRulesApplier : IInitializable, IDisposable
    {
        private readonly AnimalCollisionsObserver _collisionsObserver;
        private readonly IReadOnlyList<ICollisionRule> _rules;

        public CollisionRulesApplier(AnimalCollisionsObserver collisionsObserver, IEnumerable<ICollisionRule> rules)
        {
            _collisionsObserver = collisionsObserver;
            _rules = rules.ToList();
        }

        public void Initialize()
        {
            _collisionsObserver.Collided += Apply;
        }

        public void Dispose()
        {
            _collisionsObserver.Collided -= Apply;
        }

        /// <summary>
        /// An animal that has died in this physics step can still touch another one, so dead animals are skipped.
        /// </summary>
        public void Apply(AnimalModel first, AnimalModel second)
        {
            if (first.HealthModel.IsDead || second.HealthModel.IsDead)
                return;

            foreach (var rule in _rules)
            {
                if (rule.Apply(first, second) == CollisionRuleResult.Stop)
                    return;
            }
        }
    }
}
