using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.SharedModule.Runtime.Scripts.Services;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Wander
{
    public class WanderBehaviourFactory : AnimalBehaviourFactory<WanderConfig>
    {
        private readonly IRandomService _randomService;

        public WanderBehaviourFactory(IRandomService randomService)
        {
            _randomService = randomService;
        }

        protected override IAnimalBehaviour Create(WanderConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents) =>
            new WanderBehaviour(config, model.MovementModel, _randomService);
    }
}
