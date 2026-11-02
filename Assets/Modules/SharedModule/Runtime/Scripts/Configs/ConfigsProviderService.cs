using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.Loading;
using Object = UnityEngine.Object;

namespace Modules.SharedModule.Runtime.Scripts.Configs
{
    public sealed class ConfigsProviderService : IDisposable
    {
        private readonly HashedAssetProvider _hashedAssetProvider;

        public ConfigsProviderService(HashedAssetProvider hashedAssetProvider)
        {
            _hashedAssetProvider = hashedAssetProvider;
        }

        public void Dispose()
        {
            _hashedAssetProvider.Dispose();
        }

        public async UniTask<TConfig> GetConfigAsync<TConfig>() where TConfig : Object
        {
            var scope = typeof(TConfig).GetCustomAttribute<ConfigScopeAttribute>()?.Scope;

            if (string.IsNullOrWhiteSpace(scope))
                throw new InvalidOperationException($"{typeof(TConfig).Name} has no {nameof(ConfigScopeAttribute)}");

            var config = await _hashedAssetProvider.GetOrLoadAndRegisterObjectAsync<TConfig>(scope, shouldCreate: false);

            if (config == null)
                throw new InvalidOperationException(
                    $"{typeof(TConfig).Name} is not found by the Addressables key \"{scope}\"");

            return config;
        }
    }
}
