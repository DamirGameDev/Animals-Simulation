using System;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.QoL;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.SharedModule.Runtime.Scripts.Loading
{
    public interface IAssetLoader
    {
        UniTask<GameObject> GetInstantiatedGameObjectAsync(string assetId);

        UniTask<T> GetLoadedAssetAsync<T>(string assetId, DataContainer<Action> releaseActionContainer)
            where T : Object;
    }
}
