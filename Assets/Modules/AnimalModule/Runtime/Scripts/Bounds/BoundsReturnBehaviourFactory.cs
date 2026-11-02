using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.SharedModule.Runtime.Scripts.Services;

namespace Modules.AnimalModule.Runtime.Scripts.Bounds
{
    public class BoundsReturnBehaviourFactory : AnimalBehaviourFactory<BoundsReturnConfig>
    {
        private readonly IScreenBounds _screenBounds;
        private readonly IRandomService _randomService;

        public BoundsReturnBehaviourFactory(IScreenBounds screenBounds, IRandomService randomService)
        {
            _screenBounds = screenBounds;
            _randomService = randomService;
        }

        protected override IAnimalBehaviour Create(BoundsReturnConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents) =>
            new BoundsReturnBehaviour(config, model.MovementModel, serializableComponents.transform, _screenBounds,
                _randomService);
    }
}
