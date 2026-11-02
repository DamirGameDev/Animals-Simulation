using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Bounds
{
    public interface IScreenBounds
    {
        /// <summary>Ground point in the middle of the screen.</summary>
        Vector3 Center { get; }

        /// <summary>
        /// Whether the position is on the screen at least viewportMargin away from its edges, in viewport units.
        /// </summary>
        bool Contains(Vector3 position, float viewportMargin);

        /// <summary>Ground point under the given viewport point, (0, 0) is the bottom left corner of the screen.</summary>
        Vector3 GetGroundPoint(Vector2 viewportPoint);
    }
}
