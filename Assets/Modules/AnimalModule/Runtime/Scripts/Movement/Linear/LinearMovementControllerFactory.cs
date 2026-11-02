using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Linear
{
    public class LinearMovementControllerFactory : AnimalBehaviourFactory<LinearMovementConfig>
    {
        protected override IAnimalBehaviour Create(LinearMovementConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents) =>
            new LinearMovementController(config, model.MovementModel, serializableComponents.Rigidbody);
    }
}
