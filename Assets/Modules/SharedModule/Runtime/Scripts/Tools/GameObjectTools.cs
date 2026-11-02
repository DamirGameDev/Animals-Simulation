using UnityEngine;

namespace Modules.SharedModule.Runtime.Scripts.Tools
{
    public static class GameObjectTools
    {
        public static T GetOrAddComponent<T>(this GameObject instance) where T : Component =>
            instance.TryGetComponent<T>(out var component) ? component : instance.AddComponent<T>();
    }
}
