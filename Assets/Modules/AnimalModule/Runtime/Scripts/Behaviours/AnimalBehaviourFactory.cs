using System;
using Modules.AnimalModule.Runtime.Scripts.Animal;

namespace Modules.AnimalModule.Runtime.Scripts.Behaviours
{
    /// <summary>
    /// Base for a behaviour factory bound to one config type. It gets the whole animal,
    /// but should hand its behaviour only the parts the behaviour needs (for example MovementModel).
    /// </summary>
    public abstract class AnimalBehaviourFactory<TConfig> : IAnimalBehaviourFactory
        where TConfig : IAnimalBehaviourConfig
    {
        public Type ConfigType => typeof(TConfig);

        public IAnimalBehaviour Create(IAnimalBehaviourConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents) =>
            Create((TConfig)config, model, serializableComponents);

        protected abstract IAnimalBehaviour Create(TConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents);
    }
}
