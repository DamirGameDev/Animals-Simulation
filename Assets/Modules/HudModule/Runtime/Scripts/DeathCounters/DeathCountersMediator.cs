using System;
using Modules.AnimalModule.Runtime.Scripts.Animal;
using Modules.AnimalModule.Runtime.Scripts.Death;
using VContainer.Unity;

namespace Modules.HudModule.Runtime.Scripts.DeathCounters
{
    /// <summary>
    /// Counts every animal death by the role of the victim.
    /// </summary>
    public class DeathCountersMediator : IInitializable, IDisposable
    {
        private readonly AnimalDeathsObserver _deathsObserver;
        private readonly DeathCountersModel _deathCountersModel;

        public DeathCountersMediator(AnimalDeathsObserver deathsObserver, DeathCountersModel deathCountersModel)
        {
            _deathsObserver = deathsObserver;
            _deathCountersModel = deathCountersModel;
        }

        public void Initialize()
        {
            _deathsObserver.Died += Count;
        }

        public void Dispose()
        {
            _deathsObserver.Died -= Count;
        }

        private void Count(AnimalController victim, AnimalController killer)
        {
            _deathCountersModel.Increment(victim.Model.Config.FoodChain.Role);
        }
    }
}
