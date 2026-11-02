using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.AnimalModule.Runtime.Scripts.Movement;
using Modules.SharedModule.Runtime.Scripts.Services;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Bounds
{
    /// <summary>
    /// Turns an animal that left the screen back towards the center, with a small random deviation
    /// so that returning animals don't all line up.
    /// </summary>
    public class BoundsReturnBehaviour : IAnimalBehaviour
    {
        private readonly BoundsReturnConfig _config;
        private readonly MovementModel _movementModel;
        private readonly Transform _transform;
        private readonly IScreenBounds _screenBounds;
        private readonly IRandomService _randomService;

        public BoundsReturnBehaviour(BoundsReturnConfig config, MovementModel movementModel, Transform transform,
            IScreenBounds screenBounds, IRandomService randomService)
        {
            _config = config;
            _movementModel = movementModel;
            _transform = transform;
            _screenBounds = screenBounds;
            _randomService = randomService;
        }

        public void FixedTick(float deltaTime)
        {
            var position = _transform.position;

            if (_screenBounds.Contains(position, _config.ViewportMargin))
                return;

            var toCenter = _screenBounds.Center - position;
            toCenter.y = 0;

            var isAlreadyReturning = Vector3.Dot(_movementModel.Direction, toCenter) > 0;

            if (isAlreadyReturning)
                return;

            var maxDeviation = _config.MaxDeviationInDegrees;
            var deviation = Quaternion.Euler(0, _randomService.Range(-maxDeviation, maxDeviation), 0);
            _movementModel.SetDirection(deviation * toCenter);
        }
    }
}
