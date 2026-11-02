using Cysharp.Threading.Tasks;
using Modules.AnimalModule.Runtime.Scripts;
using Modules.HudModule.Runtime.Scripts;
using Modules.SharedModule.Runtime.Scripts.Loading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.AppModule.Runtime.Scripts.Infrastructure
{
    /// <summary>
    /// Child of PersistentServicesScope. Built by GameplayGameState after the game scene is loaded
    /// and disposed on exit, so every game service is recreated for a new game.
    /// </summary>
    public class GameServicesScope : LifetimeScope
    {
        private const string CameraAssetId = "GameCamera";

        protected override async UniTask ConfigureAsync(IContainerBuilder builder)
        {
            await RegisterCameraAsync(builder);

            AnimalModuleDependencies.Register(builder);
            HudModuleDependencies.Register(builder);
        }

        private async UniTask RegisterCameraAsync(IContainerBuilder builder)
        {
            var cameraProvider = Parent.Container.Resolve<HashedAssetProvider>();
            var camera = await cameraProvider.GetOrLoadAndRegisterObjectAsync<Camera>(CameraAssetId);

            builder.RegisterInstance(camera);
            builder.RegisterDisposeCallback(_ => cameraProvider.Dispose());
        }
    }
}
