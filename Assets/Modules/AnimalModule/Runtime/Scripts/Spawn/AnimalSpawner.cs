using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.SharedModule.Runtime.Scripts.Configs;
using Modules.SharedModule.Runtime.Scripts.Loading;
using Modules.SharedModule.Runtime.Scripts.Services;
using UnityEngine;
using VContainer.Unity;

namespace Modules.AnimalModule.Runtime.Scripts.Spawn
{
    /// <summary>
    /// Spawns a random animal from the config every few seconds until the game scope is disposed.
    /// Skips a spawn while the field already has the maximum number of animals. Each kind of animal is loaded
    /// on its first spawn and released together with the spawner. A failed spawn is logged and the next one
    /// goes on, so one broken animal doesn't stop the others.
    /// </summary>
    public class AnimalSpawner : IAsyncStartable, IDisposable
    {
        private readonly ConfigsProviderService _configsProviderService;
        private readonly HashedAssetProvider _animalConfigsProvider;
        private readonly AnimalFactory _animalFactory;
        private readonly SpawnPositionProvider _spawnPositionProvider;
        private readonly IRandomService _randomService;
        private readonly AnimalsRepository _animalsRepository;

        public AnimalSpawner(ConfigsProviderService configsProviderService,
            HashedAssetProvider animalConfigsProvider, AnimalFactory animalFactory,
            SpawnPositionProvider spawnPositionProvider, IRandomService randomService,
            AnimalsRepository animalsRepository)
        {
            _configsProviderService = configsProviderService;
            _animalConfigsProvider = animalConfigsProvider;
            _animalFactory = animalFactory;
            _spawnPositionProvider = spawnPositionProvider;
            _randomService = randomService;
            _animalsRepository = animalsRepository;
        }

        public void Dispose()
        {
            _animalConfigsProvider.Dispose();
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var config = await _configsProviderService.GetConfigAsync<AnimalsSpawnConfig>();

            if (!config.IsValid(out var error))
            {
                Debug.LogError($"{config.name}: {error}. Animals won't spawn", config);
                return;
            }

            while (!cancellation.IsCancellationRequested)
            {
                var interval = _randomService.Range(config.MinSpawnInterval, config.MaxSpawnInterval);
                var isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: cancellation)
                    .SuppressCancellationThrow();

                if (isCancelled)
                    return;

                if (_animalsRepository.ValueByKey.Count < config.MaxAnimalsCount)
                    await TrySpawnAsync(config, cancellation);
            }
        }

        private async UniTask TrySpawnAsync(AnimalsSpawnConfig config, CancellationToken cancellation)
        {
            try
            {
                await SpawnAsync(config, cancellation);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }

        private async UniTask SpawnAsync(AnimalsSpawnConfig config, CancellationToken cancellation)
        {
            var animalReference = config.Animals[_randomService.Range(0, config.Animals.Count)];
            var animalConfig = await _animalConfigsProvider.GetOrLoadAndRegisterObjectAsync<AnimalConfig>(
                animalReference, shouldCreate: false);

            if (cancellation.IsCancellationRequested)
                return;

            if (animalConfig == null)
            {
                Debug.LogError($"{config.name}: an animal config is not found by {animalReference.RuntimeKey}", config);
                return;
            }

            var position = _spawnPositionProvider.GetRandomPosition(config.ViewportMargin, config.SpawnHeight,
                config.SpawnClearanceRadius);

            await _animalFactory.CreateAsync(animalConfig, position, cancellation);
        }
    }
}
