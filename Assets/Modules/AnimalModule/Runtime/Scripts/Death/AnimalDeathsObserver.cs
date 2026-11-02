using System;
using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts.Death
{
    /// <summary>
    /// Gathers the deaths of every animal in the repository into one event, so listeners depend on it
    /// explicitly instead of subscribing to each animal themselves.
    /// </summary>
    public class AnimalDeathsObserver : IInitializable, IDisposable
    {
        /// <summary>
        /// Victim and killer. The killer is null when the animal died not because it was eaten.
        /// </summary>
        public event Action<AnimalController, AnimalController> Died;

        private readonly AnimalsRepository _animalsRepository;

        private readonly Dictionary<AnimalModel, AnimalDeathSubscription> _subscriptionsByModel =
            new Dictionary<AnimalModel, AnimalDeathSubscription>();

        public AnimalDeathsObserver(AnimalsRepository animalsRepository)
        {
            _animalsRepository = animalsRepository;
        }

        public void Initialize()
        {
            _animalsRepository.Added += Subscribe;
            _animalsRepository.Removed += Unsubscribe;
        }

        public void Dispose()
        {
            _animalsRepository.Added -= Subscribe;
            _animalsRepository.Removed -= Unsubscribe;

            foreach (var subscription in _subscriptionsByModel.Values)
                subscription.Dispose();

            _subscriptionsByModel.Clear();
        }

        private void Subscribe(AnimalModel model, AnimalController animal)
        {
            _subscriptionsByModel.Add(model, new AnimalDeathSubscription(animal, InvokeDied));
        }

        private void Unsubscribe(AnimalModel model, AnimalController animal)
        {
            if (_subscriptionsByModel.Remove(model, out var subscription))
                subscription.Dispose();
        }

        private void InvokeDied(AnimalController victim)
        {
            var killerModel = victim.Model.Killer;
            var killer = killerModel != null && _animalsRepository.TryGetValue(killerModel, out var killerAnimal)
                ? killerAnimal
                : null;

            Died?.Invoke(victim, killer);
        }
    }
}
