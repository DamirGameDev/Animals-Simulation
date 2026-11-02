using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.AppModule.Runtime.Scripts.GameStateMachinePart
{
    /// <summary>
    /// One-time application setup. Which state comes next is decided by GameStateMachineLauncher.
    /// </summary>
    public class BootstrapGameState : IGameState
    {
        public UniTask EnterAsync()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            return UniTask.CompletedTask;
        }

        public UniTask ExitAsync() => UniTask.CompletedTask;
    }
}
