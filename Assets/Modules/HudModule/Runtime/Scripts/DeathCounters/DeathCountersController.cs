using System;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;

namespace Modules.HudModule.Runtime.Scripts.DeathCounters
{
    /// <summary>
    /// Shows the death counters model in the HUD.
    /// </summary>
    public class DeathCountersController : IDisposable
    {
        private readonly DeathCountersModel _model;
        private readonly DeathCountersSerializableComponents _serializableComponents;

        public DeathCountersController(DeathCountersModel model,
            DeathCountersSerializableComponents serializableComponents)
        {
            _model = model;
            _serializableComponents = serializableComponents;

            UpdateText(FoodChainRole.Prey, _model.GetCount(FoodChainRole.Prey));
            UpdateText(FoodChainRole.Predator, _model.GetCount(FoodChainRole.Predator));

            _model.Changed += UpdateText;
        }

        public void Dispose()
        {
            _model.Changed -= UpdateText;
        }

        private void UpdateText(FoodChainRole role, int count)
        {
            switch (role)
            {
                case FoodChainRole.Prey:
                    _serializableComponents.DeadPreysText.text = $"Dead preys: {count}";
                    break;
                case FoodChainRole.Predator:
                    _serializableComponents.DeadPredatorsText.text = $"Dead predators: {count}";
                    break;
            }
        }
    }
}
