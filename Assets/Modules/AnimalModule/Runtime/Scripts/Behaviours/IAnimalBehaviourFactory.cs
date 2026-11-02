using System;
using Modules.AnimalModule.Runtime.Scripts.Animal;

namespace Modules.AnimalModule.Runtime.Scripts.Behaviours
{
    public interface IAnimalBehaviourFactory
    {
        Type ConfigType { get; }

        IAnimalBehaviour Create(IAnimalBehaviourConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents);
    }
}
