using System;
using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using UnityEngine;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts.Collisions
{
    /// <summary>
    /// Gathers the collisions of every animal in the repository with other animals into one event.
    /// Both animals get a physics callback for the same contact, and only one of them reports it,
    /// so the event comes once per contact.
    /// </summary>
    public class AnimalCollisionsObserver : IInitializable, IDisposable
    {
        public event Action<AnimalModel, AnimalModel> Collided;

        private readonly AnimalsRepository _animalsRepository;

        private readonly Dictionary<AnimalSerializableComponents, Action<Collision>> _collisionHandlers =
            new Dictionary<AnimalSerializableComponents, Action<Collision>>();

        public AnimalCollisionsObserver(AnimalsRepository animalsRepository)
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

            foreach (var (serializableComponents, handler) in _collisionHandlers)
                serializableComponents.CollisionObserver.Entered -= handler;

            _collisionHandlers.Clear();
        }

        private void Subscribe(AnimalModel model, AnimalController animal)
        {
            Action<Collision> handler = collision => InvokeCollided(animal, collision);
            var serializableComponents = animal.SerializableComponents;

            _collisionHandlers.Add(serializableComponents, handler);
            serializableComponents.CollisionObserver.Entered += handler;
        }

        private void Unsubscribe(AnimalModel model, AnimalController animal)
        {
            var serializableComponents = animal.SerializableComponents;

            if (_collisionHandlers.Remove(serializableComponents, out var handler))
                serializableComponents.CollisionObserver.Entered -= handler;
        }

        private void InvokeCollided(AnimalController animal, Collision collision)
        {
            if (collision.rigidbody == null ||
                !collision.rigidbody.TryGetComponent<AnimalSerializableComponents>(out var otherSerializableComponents) ||
                !IsReporter(animal.SerializableComponents, otherSerializableComponents) ||
                !_animalsRepository.TryGetByView(otherSerializableComponents, out var other))
                return;

            Collided?.Invoke(animal.Model, other.Model);
        }

        private static bool IsReporter(AnimalSerializableComponents self, AnimalSerializableComponents other) =>
            self.GetInstanceID() < other.GetInstanceID();
    }
}
