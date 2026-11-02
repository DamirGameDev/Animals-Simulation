using System.Collections.Generic;
using Modules.SharedModule.Runtime.Scripts.Repository;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    /// <summary>
    /// Live animals by their model. Physics knows only the view it touched, so it finds the animal through TryGetByView.
    /// Disposing the repository disposes the animals that are still alive.
    /// </summary>
    public class AnimalsRepository : IndexRepository<AnimalModel, AnimalController>
    {
        private readonly Dictionary<AnimalSerializableComponents, AnimalController> _animalsByView =
            new Dictionary<AnimalSerializableComponents, AnimalController>();

        public AnimalsRepository()
        {
            Added += (_, animal) => _animalsByView.Add(animal.SerializableComponents, animal);
            Removed += (_, animal) => _animalsByView.Remove(animal.SerializableComponents);
        }

        public bool TryGetByView(AnimalSerializableComponents view, out AnimalController animal) =>
            _animalsByView.TryGetValue(view, out animal);

        public override void Dispose()
        {
            foreach (var animal in ValueByKey.Values)
                animal.Dispose();

            base.Dispose();
            _animalsByView.Clear();
        }
    }
}
