using Cysharp.Threading.Tasks;

namespace Modules.AppModule.Runtime.Scripts.GameStateMachinePart
{
    public interface IGameState
    {
        UniTask EnterAsync();
        UniTask ExitAsync();
    }
}
