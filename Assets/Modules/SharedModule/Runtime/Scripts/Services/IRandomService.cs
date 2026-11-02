namespace Modules.SharedModule.Runtime.Scripts.Services
{
    /// <summary>
    /// Injected instead of calling UnityEngine.Random directly, so gameplay rules stay deterministic in tests.
    /// </summary>
    public interface IRandomService
    {
        float Range(float minInclusive, float maxInclusive);
        int Range(int minInclusive, int maxExclusive);
    }
}
