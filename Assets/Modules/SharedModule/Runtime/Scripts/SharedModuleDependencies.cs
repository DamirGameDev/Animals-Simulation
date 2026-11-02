using Modules.SharedModule.Runtime.Scripts.Configs;
using Modules.SharedModule.Runtime.Scripts.Loading;
using Modules.SharedModule.Runtime.Scripts.Services;
using VContainer;

namespace Modules.SharedModule.Runtime.Scripts
{
    /// <summary>
    /// Everything SharedModule puts into a container. Registered once, for the whole app.
    /// </summary>
    public static class SharedModuleDependencies
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<IAssetLoader, AssetFromAddressablesLoader>(Lifetime.Singleton);
            builder.Register<HashedAssetProvider>(Lifetime.Transient);
            builder.Register<ConfigsProviderService>(Lifetime.Singleton);
            builder.Register<IRandomService, UnityRandomService>(Lifetime.Singleton);
        }
    }
}
