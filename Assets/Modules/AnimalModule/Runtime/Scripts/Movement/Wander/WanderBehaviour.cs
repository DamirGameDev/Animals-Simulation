using Modules.AnimalModule.Runtime.Scripts.Behaviours;
using Modules.SharedModule.Runtime.Scripts.Services;
using UnityEngine;

namespace Modules.AnimalModule.Runtime.Scripts.Movement.Wander
{
    /// <summary>
    /// Turns the direction by a random angle at an interval, so the animal moves randomly and not in a straight line.
    /// </summary>
    public class WanderBehaviour : IAnimalBehaviour
    {
        private readonly WanderConfig _config;
        private readonly MovementModel _movementModel;
        private readonly IRandomService _randomService;

        private float _elapsedTime;

        public WanderBehaviour(WanderConfig config, MovementModel movementModel, IRandomService randomService)
        {
            _config = config;
            _movementModel = movementModel;
            _randomService = randomService;
        }

        public void FixedTick(float deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_elapsedTime < _config.Interval)
                return;

            _elapsedTime = 0;

            var maxTurn = _config.MaxTurnInDegrees;
            var turn = Quaternion.Euler(0, _randomService.Range(-maxTurn, maxTurn), 0);
            _movementModel.SetDirection(turn * _movementModel.Direction);
        }
    }
}
