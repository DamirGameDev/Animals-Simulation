using System;
using System.Collections.Generic;
using System.Linq;
using Modules.AnimalModule.Runtime.Scripts.Animal;

namespace Modules.AnimalModule.Runtime.Scripts.Behaviours
{
    /// <summary>
    /// Registry "behaviour config type → factory". Builds the behaviours of an animal from the list in its config,
    /// keeping the list order, which is also the order they tick in.
    /// </summary>
    public class AnimalBehavioursFactory
    {
        private readonly IReadOnlyDictionary<Type, IAnimalBehaviourFactory> _factoriesByConfigType;

        public AnimalBehavioursFactory(IEnumerable<IAnimalBehaviourFactory> factories)
        {
            _factoriesByConfigType = factories.ToDictionary(factory => factory.ConfigType);
        }

        public IReadOnlyList<IAnimalBehaviour> Create(IReadOnlyList<IAnimalBehaviourConfig> configs, AnimalModel model,
            AnimalSerializableComponents serializableComponents)
        {
            var behaviours = new IAnimalBehaviour[configs.Count];

            for (var index = 0; index < configs.Count; index++)
                behaviours[index] = Create(configs[index], model, serializableComponents);

            return behaviours;
        }

        private IAnimalBehaviour Create(IAnimalBehaviourConfig config, AnimalModel model,
            AnimalSerializableComponents serializableComponents)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config), "An animal config has an empty behaviour");

            if (!_factoriesByConfigType.TryGetValue(config.GetType(), out var factory))
                throw new InvalidOperationException($"No {nameof(IAnimalBehaviourFactory)} for {config.GetType().Name}");

            return factory.Create(config, model, serializableComponents);
        }
    }
}
