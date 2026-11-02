using Modules.AnimalModule.Runtime.Scripts.Bounds;
using Modules.SharedModule.Runtime.Scripts.Services;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Spawn
{
    /// <summary>
    /// Random point on screen, preferably away from other animals so a new one doesn't appear inside them.
    /// </summary>
    public class SpawnPositionProvider
    {
        private const int MaxAttempts = 10;

        private readonly IScreenBounds _screenBounds;
        private readonly IRandomService _randomService;
        private readonly Collider[] _overlapBuffer = new Collider[8];

        public SpawnPositionProvider(IScreenBounds screenBounds, IRandomService randomService)
        {
            _screenBounds = screenBounds;
            _randomService = randomService;
        }

        /// <summary>
        /// When every attempt hits an occupied spot, the last one is returned: a crowded field doesn't stop spawning.
        /// </summary>
        public Vector3 GetRandomPosition(float viewportMargin, float height, float clearanceRadius)
        {
            var position = Vector3.zero;

            for (var attempt = 0; attempt < MaxAttempts; attempt++)
            {
                var viewportPoint = new Vector2(
                    _randomService.Range(viewportMargin, 1 - viewportMargin),
                    _randomService.Range(viewportMargin, 1 - viewportMargin));

                position = _screenBounds.GetGroundPoint(viewportPoint) + Vector3.up * height;

                if (IsFree(position, clearanceRadius))
                    break;
            }

            return position;
        }
        
        private bool IsFree(Vector3 position, float clearanceRadius)
        {
            var count = Physics.OverlapSphereNonAlloc(position, clearanceRadius, _overlapBuffer, Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            for (var index = 0; index < count; index++)
            {
                if (_overlapBuffer[index].attachedRigidbody != null)
                    return false;
            }

            return true;
        }
    }
}
