using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    /// <summary>
    /// One physics tick for all animals instead of a MonoBehaviour update per animal.
    /// While the game is torn down, the scene with the views can go before the game scope,
    /// so an animal whose view is already destroyed is skipped.
    /// </summary>
    public class AnimalsTicker : IFixedTickable
    {
        private readonly AnimalsRepository _animalsRepository;
        private readonly List<AnimalController> _tickedAnimals = new List<AnimalController>();

        public AnimalsTicker(AnimalsRepository animalsRepository)
        {
            _animalsRepository = animalsRepository;
        }

        public void FixedTick()
        {
            _tickedAnimals.Clear();
            _tickedAnimals.AddRange(_animalsRepository.ValueByKey.Values);

            foreach (var animal in _tickedAnimals)
            {
                if (animal.SerializableComponents != null)
                    animal.FixedTick(Time.fixedDeltaTime);
            }
        }
    }
}
