using System;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Pool;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts.Death
{
    /// <summary>
    /// Removes a dead animal from the game and returns its view to the pool.
    /// </summary>
    public class AnimalDespawner : IInitializable, IDisposable
    {
        private readonly AnimalDeathsObserver _deathsObserver;
        private readonly AnimalsRepository _animalsRepository;
        private readonly AnimalSerializableComponentsPool _serializableComponentsPool;

        public AnimalDespawner(AnimalDeathsObserver deathsObserver, AnimalsRepository animalsRepository,
            AnimalSerializableComponentsPool serializableComponentsPool)
        {
            _deathsObserver = deathsObserver;
            _animalsRepository = animalsRepository;
            _serializableComponentsPool = serializableComponentsPool;
        }

        public void Initialize()
        {
            _deathsObserver.Died += Despawn;
        }

        public void Dispose()
        {
            _deathsObserver.Died -= Despawn;
        }

        private void Despawn(AnimalController victim, AnimalController killer)
        {
            _animalsRepository.RemoveByValue(victim);
            victim.Dispose();
            _serializableComponentsPool.Release(victim.Model.Config, victim.SerializableComponents);
        }
    }
}
