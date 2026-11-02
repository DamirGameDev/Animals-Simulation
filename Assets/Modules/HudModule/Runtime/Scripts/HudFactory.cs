using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.HudModule.Runtime.Scripts.DeathCounters;
using Modules.HudModule.Runtime.Scripts.TastyLabel;
using Modules.SharedModule.Runtime.Scripts.Configs;
using Modules.SharedModule.Runtime.Scripts.Loading;
using UnityEngine;
using VContainer.Unity;

namespace Modules.HudModule.Runtime.Scripts
{
    /// <summary>
    /// Creates the HUD when the game starts and owns its controllers. The asset provider owns only the loaded
    /// assets and the HUD instance.
    /// </summary>
    public class HudFactory : IAsyncStartable, IDisposable
    {
        private const string HudAssetId = "Hud";

        private readonly HashedAssetProvider _hashedAssetProvider;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly DeathCountersModel _deathCountersModel;
        private readonly TastyLabelsMediator _tastyLabelsMediator;
        private readonly Camera _camera;

        private DeathCountersController _deathCountersController;
        private TastyLabelsController _tastyLabelsController;

        public HudFactory(HashedAssetProvider hashedAssetProvider, ConfigsProviderService configsProviderService,
            DeathCountersModel deathCountersModel, TastyLabelsMediator tastyLabelsMediator, Camera camera)
        {
            _hashedAssetProvider = hashedAssetProvider;
            _configsProviderService = configsProviderService;
            _deathCountersModel = deathCountersModel;
            _tastyLabelsMediator = tastyLabelsMediator;
            _camera = camera;
        }

        /// <summary>
        /// Controllers are disposed right away, so they stop listening together with the game scope;
        /// the view goes after them.
        /// </summary>
        public void Dispose()
        {
            _tastyLabelsController?.Dispose();
            _deathCountersController?.Dispose();
            _hashedAssetProvider.Dispose();
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var config = await _configsProviderService.GetConfigAsync<HudConfig>();
            var hud = await _hashedAssetProvider.GetOrLoadAndRegisterObjectAsync<HudSerializableComponents>(HudAssetId);

            if (cancellation.IsCancellationRequested)
                return;

            _deathCountersController = new DeathCountersController(_deathCountersModel, hud.DeathCounters);

            _tastyLabelsController =
                new TastyLabelsController(_camera, hud.Canvas, config, hud.TastyLabelPrefab, hud.TastyLabelsRoot);
            _tastyLabelsMediator.Bind(_tastyLabelsController);
        }
    }
}
