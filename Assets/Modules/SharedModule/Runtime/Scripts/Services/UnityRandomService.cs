namespace Modules.SharedModule.Runtime.Scripts.Services
{
    public class UnityRandomService : IRandomService
    {
        public float Range(float minInclusive, float maxInclusive) =>
            UnityEngine.Random.Range(minInclusive, maxInclusive);

        public int Range(int minInclusive, int maxExclusive) =>
            UnityEngine.Random.Range(minInclusive, maxExclusive);
    }
}
