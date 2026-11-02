using System;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.Observers;
using Modules.SharedModule.Runtime.Scripts.QoL;
using Modules.SharedModule.Runtime.Scripts.Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Modules.SharedModule.Runtime.Scripts.Loading
{
    public class AssetFromAddressablesLoader : IAssetLoader
    {
        public async UniTask<GameObject> GetInstantiatedGameObjectAsync(string assetId)
        {
            var handle = Addressables.InstantiateAsync(assetId);

            var instance = await handle;

            if (!instance)
                throw new NullReferenceException($"Addressable asset with assetId {assetId} is not found");

            instance.GetOrAddComponent<DestroyObserver>().Destroyed += () => Addressables.ReleaseInstance(handle);

            return instance;
        }

        public async UniTask<T> GetLoadedAssetAsync<T>(string assetId, DataContainer<Action> releaseActionContainer)
            where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(assetId);

            releaseActionContainer.Data = ReleaseAsset;

            return await handle;

            void ReleaseAsset()
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }
    }
}
