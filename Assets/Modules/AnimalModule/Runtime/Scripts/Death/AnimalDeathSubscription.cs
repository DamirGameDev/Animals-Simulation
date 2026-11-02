using System;
using Modules.AnimalModule.Runtime.Scripts.Animal;

namespace Modules.AnimalModule.Runtime.Scripts.Death
{
    /// <summary>
    /// Subscription to the death of one animal. Unsubscribes on Dispose.
    /// </summary>
    public class AnimalDeathSubscription : IDisposable
    {
        private readonly AnimalController _animal;
        private readonly Action<AnimalController> _died;

        public AnimalDeathSubscription(AnimalController animal, Action<AnimalController> died)
        {
            _animal = animal;
            _died = died;

            _animal.Model.HealthModel.Died += InvokeDied;
        }

        public void Dispose()
        {
            _animal.Model.HealthModel.Died -= InvokeDied;
        }

        private void InvokeDied()
        {
            _died.Invoke(_animal);
        }
    }
}
