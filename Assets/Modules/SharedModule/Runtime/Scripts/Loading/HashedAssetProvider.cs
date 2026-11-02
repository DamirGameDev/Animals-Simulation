using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.QoL;
using Modules.SharedModule.Runtime.Scripts.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Modules.SharedModule.Runtime.Scripts.Loading
{
    /// <summary>
    /// Loads an asset or instantiates a prefab once per key and caches it.
    /// Disposing destroys the instances and releases the loaded assets.
    /// </summary>
    public class HashedAssetProvider : IDisposable
    {
        private readonly Dictionary<string, Object> _objectsByAssetId = new Dictionary<string, Object>();
        private readonly HashSet<string> _loadingAssetIds = new HashSet<string>();
        private readonly List<Action> _releaseActions = new List<Action>();

        private readonly IAssetLoader _assetLoader;

        public HashedAssetProvider(IAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public void Dispose()
        {
            DisposeAsync().Forget();
        }

        public UniTask<T> GetOrLoadAndRegisterObjectAsync<T>(AssetReference reference, bool shouldCreate = true)
            where T : Object =>
            GetOrLoadAndRegisterObjectAsync<T>(reference.RuntimeKey.ToString(), shouldCreate);

        public async UniTask<T> GetOrLoadAndRegisterObjectAsync<T>(string assetId, bool shouldCreate = true)
            where T : Object
        {
            var cached = await GetCachedAsync<T>(assetId);

            if (cached)
                return cached;

            var releaseActionContainer = new DataContainer<Action>();

            _loadingAssetIds.Add(assetId);
            T loaded;

            try
            {
                loaded = shouldCreate && (typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(GameObject))
                    ? await AssetProvider.InstantiateAsync<T>(assetId, _assetLoader)
                    : await AssetProvider.LoadAsync<T>(assetId, _assetLoader, releaseActionContainer);
            }
            finally
            {
                _loadingAssetIds.Remove(assetId);
            }

            if (loaded == null)
            {
                releaseActionContainer.Data?.Invoke();
                return null;
            }

            _objectsByAssetId[assetId] = loaded;

            if (releaseActionContainer.Data != null)
                _releaseActions.Add(releaseActionContainer.Data);

            return loaded;
        }

        public async UniTask DisposeAsync()
        {
            await AsyncTools.WaitWhileWithoutSkippingFrame(() => _loadingAssetIds.Count > 0);

            var instances = _objectsByAssetId.Values
                .OfType<Component>()
                .Where(instance => instance && instance.gameObject.scene.isLoaded)
                .Select(instance => instance.gameObject)
                .ToHashSet();

            foreach (var instance in instances)
                Object.Destroy(instance);

            foreach (var releaseAction in _releaseActions)
                releaseAction.Invoke();

            _objectsByAssetId.Clear();
            _releaseActions.Clear();
        }

        private async UniTask<T> GetCachedAsync<T>(string assetId) where T : Object
        {
            await AsyncTools.WaitWhileWithoutSkippingFrame(() => _loadingAssetIds.Contains(assetId));
            return _objectsByAssetId.TryGetValue(assetId, out var cached) ? cached as T : null;
        }
    }
}
