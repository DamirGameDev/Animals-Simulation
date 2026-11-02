using Modules.HudModule.Runtime.Scripts.DeathCounters;
using Modules.HudModule.Runtime.Scripts.TastyLabel;
using VContainer;
using VContainer.Unity;

namespace Modules.HudModule.Runtime.Scripts
{
    /// <summary>
    /// Everything HudModule puts into the game scope.
    /// </summary>
    public static class HudModuleDependencies
    {
        public static void Register(IContainerBuilder builder)
        {
            builder.Register<DeathCountersModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<DeathCountersMediator>();
            builder.RegisterEntryPoint<TastyLabelsMediator>().AsSelf();
            builder.RegisterEntryPoint<HudFactory>();
        }
    }
}
