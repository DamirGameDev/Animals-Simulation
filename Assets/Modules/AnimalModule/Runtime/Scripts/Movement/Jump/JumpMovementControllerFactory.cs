using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Jump
{
    public class JumpMovementControllerFactory : AnimalBehaviourFactory<JumpMovementConfig>
    {
        protected override IAnimalBehaviour Create(JumpMovementConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents) =>
            new JumpMovementController(config, model.MovementModel, serializableComponents.Rigidbody,
                serializableComponents.Collider);
    }
}
