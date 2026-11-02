using System;
using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Death;
using VContainer.Unity;

namespace Modules.HudModule.Runtime.Scripts.TastyLabel
{
    /// <summary>
    /// Asks for a "Tasty!" label every time an animal eats. Listens from the start of the game, so meals that happen
    /// while the HUD is still loading are shown as soon as the labels controller is bound.
    /// </summary>
    public class TastyLabelsMediator : IInitializable, IDisposable
    {
        private readonly AnimalDeathsObserver _deathsObserver;
        private readonly List<AnimalController> _pendingEaters = new List<AnimalController>();

        private TastyLabelsController _labelsController;

        public TastyLabelsMediator(AnimalDeathsObserver deathsObserver)
        {
            _deathsObserver = deathsObserver;
        }

        public void Initialize()
        {
            _deathsObserver.Died += TryShow;
        }

        public void Dispose()
        {
            _deathsObserver.Died -= TryShow;
            _pendingEaters.Clear();
        }

        public void Bind(TastyLabelsController labelsController)
        {
            _labelsController = labelsController;

            foreach (var eater in _pendingEaters)
            {
                if (!eater.Model.HealthModel.IsDead)
                    _labelsController.Show(eater);
            }

            _pendingEaters.Clear();
        }

        private void TryShow(AnimalController victim, AnimalController killer)
        {
            if (killer == null)
                return;

            if (_labelsController == null)
                _pendingEaters.Add(killer);
            else
                _labelsController.Show(killer);
        }
    }
}
