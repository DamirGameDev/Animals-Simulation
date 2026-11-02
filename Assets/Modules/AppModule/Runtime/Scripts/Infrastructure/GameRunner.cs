using Modules.SharedModule.Runtime.Scripts.Holders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Modules.AppModule.Runtime.Scripts.Infrastructure
{
    /// <summary>
    /// Lets Play be pressed from any scene: the app always starts from the Initial scene with the persistent scope.
    /// </summary>
    public static class GameRunner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadInitialScene()
        {
            if (SceneManager.GetActiveScene().name != ScenesNamesHolder.Initial)
                SceneManager.LoadScene(ScenesNamesHolder.Initial);
        }
    }
}
