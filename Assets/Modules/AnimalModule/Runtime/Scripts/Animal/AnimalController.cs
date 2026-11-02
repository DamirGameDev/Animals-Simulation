using System;
using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    /// <summary>
    /// Owns the behaviours of one animal and ticks them in order.
    /// </summary>
    public class AnimalController : IDisposable
    {
        public AnimalModel Model { get; }
        public AnimalSerializableComponents SerializableComponents { get; }

        private readonly IReadOnlyList<IAnimalBehaviour> _behaviours;

        public AnimalController(AnimalModel model, AnimalSerializableComponents serializableComponents,
            IReadOnlyList<IAnimalBehaviour> behaviours)
        {
            Model = model;
            SerializableComponents = serializableComponents;
            _behaviours = behaviours;
        }

        public void Dispose()
        {
            foreach (var behaviour in _behaviours)
            {
                if (behaviour is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        public void FixedTick(float deltaTime)
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.FixedTick(deltaTime);
            }
        }
    }
}
