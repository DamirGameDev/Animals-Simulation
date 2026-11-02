using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Modules.SharedModule.Runtime.Scripts.GameStateMachine;

namespace Modules.AppModule.Runtime.Scripts.GameStateMachinePart
{
    public class GameStateMachine
    {
        private readonly IReadOnlyDictionary<GameStateType, IGameState> _statesByType;

        private IGameState _activeGameState;

        public GameStateMachine(BootstrapGameState bootstrapGameState, GameplayGameState gameplayGameState)
        {
            _statesByType = new Dictionary<GameStateType, IGameState>
            {
                [GameStateType.Bootstrap] = bootstrapGameState,
                [GameStateType.Gameplay] = gameplayGameState
            };
        }

        public async UniTask EnterAsync(GameStateType gameStateType)
        {
            if (_activeGameState != null)
                await _activeGameState.ExitAsync();

            _activeGameState = _statesByType[gameStateType];
            await _activeGameState.EnterAsync();
        }
    }
}
