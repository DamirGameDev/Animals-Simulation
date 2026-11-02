using Modules.SharedModule.Runtime.Scripts.Services;
using UnityEngine;

namespace Modules.AnimalModule.Tests.Fakes
{
    /// <summary>
    /// Returns a fixed point of the range. By default the middle of a float range, so random deviations become zero,
    /// and the lower bound of an int range.
    /// </summary>
    public class FakeRandomService : IRandomService
    {
        /// <summary>
        /// 0 is the lower bound of a float range, 1 is the upper one.
        /// </summary>
        public float FloatRangePosition { get; set; } = 0.5f;

        public int IntRangeOffset { get; set; }

        public float Range(float minInclusive, float maxInclusive) =>
            Mathf.Lerp(minInclusive, maxInclusive, FloatRangePosition);

        public int Range(int minInclusive, int maxExclusive) => minInclusive + IntRangeOffset;
    }
}
