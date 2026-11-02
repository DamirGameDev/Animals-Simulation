using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.QoL;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.SharedModule.Runtime.Scripts.Loading
{
    public static class AssetProvider
    {
        public static async UniTask<T> InstantiateAsync<T>(string assetId, IAssetLoader assetLoader) where T : Object
        {
            var instance = await assetLoader.GetInstantiatedGameObjectAsync(assetId);

            if (typeof(T) == typeof(GameObject))
                return instance as T;

            return instance.GetComponent<T>(typeof(T), assetId);
        }

        public static async UniTask<T> LoadAsync<T>(string assetId, IAssetLoader assetLoader,
            DataContainer<Action> releaseActionContainer) where T : Object
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            try
            {
                if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    var gameObject = await assetLoader.GetLoadedAssetAsync<GameObject>(assetId, releaseActionContainer);
                    return gameObject.GetComponent<T>(typeof(T), assetId);
                }

                return await assetLoader.GetLoadedAssetAsync<T>(assetId, releaseActionContainer);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                return null;
            }
        }

        private static T GetComponent<T>(this GameObject gameObject, Type type, string assetId) where T : class
        {
            var targetComponent = gameObject.GetComponents<Component>().FirstOrDefault(type.IsInstanceOfType) as T;

            if (targetComponent == null)
                throw new NullReferenceException($"Component {type} on gameObject with assetId {assetId} not found");

            return targetComponent;
        }
    }
}
