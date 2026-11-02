using Cysharp.Threading.Tasks;
using Modules.AppModule.Runtime.Scripts.GameStateMachinePart;
using Modules.SharedModule.Runtime.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.AppModule.Runtime.Scripts.Infrastructure
{
    public class PersistentServicesScope : LifetimeScope
    {
        [SerializeField] private GameServicesScope _gameServicesScope;

        protected override UniTask ConfigureAsync(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            SharedModuleDependencies.Register(builder);

            builder.RegisterInstance(_gameServicesScope);
            builder.Register<BootstrapGameState>(Lifetime.Singleton);
            builder.Register<GameplayGameState>(Lifetime.Singleton);
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameStateMachineLauncher>();

            return UniTask.CompletedTask;
        }
    }
}
