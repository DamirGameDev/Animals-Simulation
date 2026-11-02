using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    public class AnimalModel
    {
        public AnimalConfig Config { get; }
        public MovementModel MovementModel { get; }
        public HealthModel HealthModel { get; }

        /// <summary>
        /// The animal that ate this one. Null while alive or when it died for another reason.
        /// </summary>
        public AnimalModel Killer { get; private set; }

        public AnimalModel(AnimalConfig config, MovementModel movementModel, HealthModel healthModel)
        {
            Config = config;
            MovementModel = movementModel;
            HealthModel = healthModel;
        }

        /// <summary>
        /// Remembers the killer before the health runs out, so listeners of HealthModel.Died can read it.
        /// </summary>
        public void Die(AnimalModel killer)
        {
            if (HealthModel.IsDead)
                return;

            Killer = killer;
            HealthModel.Die();
        }
    }
}
