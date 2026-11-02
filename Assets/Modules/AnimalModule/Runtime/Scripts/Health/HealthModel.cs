using System;

namespace Modules.AnimalModule.Runtime.Scripts.Health
{
    /// <summary>
    /// Exactly one health point, so any lethal hit kills. Knows nothing about who killed.
    /// </summary>
    public class HealthModel
    {
        public const int MaxHealthPoints = 1;

        public int HealthPoints { get; private set; } = MaxHealthPoints;
        public bool IsDead => HealthPoints == 0;

        /// <summary>
        /// Raised once, when the health points run out.
        /// </summary>
        public event Action Died;

        /// <summary>
        /// Internal, so outside the module an animal dies only through AnimalModel.Die and the killer is never lost.
        /// </summary>
        internal void Die()
        {
            if (IsDead)
                return;

            HealthPoints = 0;
            Died?.Invoke();
        }
    }
}
