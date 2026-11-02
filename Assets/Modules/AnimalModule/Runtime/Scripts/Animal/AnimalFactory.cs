using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Health;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.AnimalModule.Runtime.Scripts.Pool;
using Modules.SharedModule.Runtime.Scripts.Services;
using Modules.SharedModule.Runtime.Scripts.Tools;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Animal
{
    public class AnimalFactory
    {
        private readonly AnimalSerializableComponentsPool _serializableComponentsPool;
        private readonly AnimalsRepository _animalsRepository;
        private readonly AnimalBehavioursFactory _behavioursFactory;
        private readonly IRandomService _randomService;

        public AnimalFactory(AnimalSerializableComponentsPool serializableComponentsPool, AnimalsRepository animalsRepository,
            AnimalBehavioursFactory behavioursFactory, IRandomService randomService)
        {
            _serializableComponentsPool = serializableComponentsPool;
            _animalsRepository = animalsRepository;
            _behavioursFactory = behavioursFactory;
            _randomService = randomService;
        }

        /// <summary>
        /// Returns null when cancelled while the prefab of the kind is loading. When the behaviours can't be built,
        /// the view goes back to the pool before the exception is rethrown.
        /// </summary>
        public async UniTask<AnimalController> CreateAsync(AnimalConfig config, Vector3 position,
            CancellationToken cancellation)
        {
            var direction = _randomService.GetDirectionOnGround();
            var serializableComponents =
                await _serializableComponentsPool.GetAsync(config, position, Quaternion.LookRotation(direction),
                    cancellation);

            if (serializableComponents == null)
                return null;

            var model = new AnimalModel(config, new MovementModel(direction), new HealthModel());
            IReadOnlyList<IAnimalBehaviour> behaviours;

            try
            {
                behaviours = _behavioursFactory.Create(config.Behaviours, model, serializableComponents);
            }
            catch
            {
                _serializableComponentsPool.Release(config, serializableComponents);
                throw;
            }

            var controller = new AnimalController(model, serializableComponents, behaviours);
            _animalsRepository.Add(model, controller);

            return controller;
        }
    }
}
