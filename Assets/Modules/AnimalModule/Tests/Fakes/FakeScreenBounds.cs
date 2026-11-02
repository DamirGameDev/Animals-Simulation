using Modules.AnimalModule.Runtime.Scripts.Bounds;
using UnityEngine;

namespace Modules.AnimalModule.Tests.Fakes
{
    /// <summary>
    /// Square of the given half size around the world origin.
    /// </summary>
    public class FakeScreenBounds : IScreenBounds
    {
        public Vector3 Center => Vector3.zero;

        private readonly float _halfSize;

        public FakeScreenBounds(float halfSize)
        {
            _halfSize = halfSize;
        }

        public bool Contains(Vector3 position, float viewportMargin)
        {
            var halfSize = _halfSize * (1 - 2 * viewportMargin);
            return Mathf.Abs(position.x) <= halfSize && Mathf.Abs(position.z) <= halfSize;
        }

        public Vector3 GetGroundPoint(Vector2 viewportPoint) =>
            new Vector3((viewportPoint.x * 2 - 1) * _halfSize, 0, (viewportPoint.y * 2 - 1) * _halfSize);
    }
}
