using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.SharedModule.Runtime.Scripts.Loading;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Modules.AnimalModule.Runtime.Scripts.Pool
{
    /// <summary>
    /// One pool per kind of animal. The prefab of a kind is loaded on its first request and released on dispose.
    /// A request cancelled while the prefab is loading returns null.
    /// </summary>
    public class AnimalSerializableComponentsPool : IDisposable
    {
        private readonly Dictionary<AnimalConfig, ObjectPool<AnimalSerializableComponents>> _poolsByConfig =
            new Dictionary<AnimalConfig, ObjectPool<AnimalSerializableComponents>>();

        private readonly HashedAssetProvider _prefabsProvider;

        private Transform _root;

        public AnimalSerializableComponentsPool(HashedAssetProvider prefabsProvider)
        {
            _prefabsProvider = prefabsProvider;
        }

        public void Dispose()
        {
            foreach (var pool in _poolsByConfig.Values)
                pool.Dispose();

            _poolsByConfig.Clear();

            if (_root != null)
                Object.Destroy(_root.gameObject);

            _prefabsProvider.Dispose();
        }

        public async UniTask<AnimalSerializableComponents> GetAsync(AnimalConfig config, Vector3 position,
            Quaternion rotation, CancellationToken cancellation)
        {
            var pool = await GetPoolAsync(config);

            if (cancellation.IsCancellationRequested)
                return null;

            var serializableComponents = pool.Get();

            serializableComponents.transform.SetPositionAndRotation(position, rotation);
            serializableComponents.gameObject.SetActive(true);

            ResetRigidbody(serializableComponents.Rigidbody, position, rotation);

            return serializableComponents;
        }

        public void Release(AnimalConfig config, AnimalSerializableComponents serializableComponents)
        {
            _poolsByConfig[config].Release(serializableComponents);
        }

        private async UniTask<ObjectPool<AnimalSerializableComponents>> GetPoolAsync(AnimalConfig config)
        {
            if (_poolsByConfig.TryGetValue(config, out var pool))
                return pool;

            var prefab = await _prefabsProvider.GetOrLoadAndRegisterObjectAsync<AnimalSerializableComponents>(
                config.Prefab, shouldCreate: false);

            if (prefab == null)
                throw new InvalidOperationException($"{config.name}: the prefab is not found by {config.Prefab.RuntimeKey}");

            if (_poolsByConfig.TryGetValue(config, out pool))
                return pool;

            pool = new ObjectPool<AnimalSerializableComponents>(
                createFunc: () => Object.Instantiate(prefab, GetRoot()),
                actionOnRelease: serializableComponents => serializableComponents.gameObject.SetActive(false),
                actionOnDestroy: serializableComponents => Object.Destroy(serializableComponents.gameObject));

            _poolsByConfig.Add(config, pool);
            return pool;
        }

        /// <summary>
        /// An interpolated rigidbody keeps its own pose and would override the transform.
        /// </summary>
        private static void ResetRigidbody(Rigidbody rigidbody, Vector3 position, Quaternion rotation)
        {
            rigidbody.position = position;
            rigidbody.rotation = rotation;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        private Transform GetRoot()
        {
            if (_root == null)
                _root = new GameObject("Animals").transform;

            return _root;
        }
    }
}
