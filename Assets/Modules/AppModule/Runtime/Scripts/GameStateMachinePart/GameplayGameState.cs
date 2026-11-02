using Cysharp.Threading.Tasks;
using Modules.AppModule.Runtime.Scripts.Infrastructure;
using Modules.SharedModule.Runtime.Scripts.Holders;
using UnityEngine.SceneManagement;

namespace Modules.AppModule.Runtime.Scripts.GameStateMachinePart
{
    public class GameplayGameState : IGameState
    {
        private readonly GameServicesScope _gameServicesScope;

        public GameplayGameState(GameServicesScope gameServicesScope)
        {
            _gameServicesScope = gameServicesScope;
        }

        public async UniTask EnterAsync()
        {
            await SceneManager.LoadSceneAsync(ScenesNamesHolder.Game).ToUniTask();
            await _gameServicesScope.BuildAsync();
        }

        public UniTask ExitAsync()
        {
            _gameServicesScope.DisposeCore();
            return UniTask.CompletedTask;
        }
    }
}
