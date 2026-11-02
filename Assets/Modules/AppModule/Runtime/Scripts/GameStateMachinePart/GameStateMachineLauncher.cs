using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.GameStateMachine;
using VContainer.Unity;

namespace Modules.AppModule.Runtime.Scripts.GameStateMachinePart
{
    /// <summary>
    /// Runs the startup sequence of states.
    /// </summary>
    public class GameStateMachineLauncher : IAsyncStartable
    {
        private readonly GameStateMachine _gameStateMachine;

        public GameStateMachineLauncher(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await _gameStateMachine.EnterAsync(GameStateType.Bootstrap);
            await _gameStateMachine.EnterAsync(GameStateType.Gameplay);
        }
    }
}
