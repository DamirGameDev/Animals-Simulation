using System;
using System.Collections.Generic;
using Modules.AnimalModule.Runtime.Scripts.FoodChain;

namespace Modules.HudModule.Runtime.Scripts.DeathCounters
{
    /// <summary>
    /// How many animals of each food chain role died in the current game.
    /// </summary>
    public class DeathCountersModel
    {
        public event Action<FoodChainRole, int> Changed;

        private readonly Dictionary<FoodChainRole, int> _countsByRole = new Dictionary<FoodChainRole, int>();

        public int GetCount(FoodChainRole role) => _countsByRole.GetValueOrDefault(role);

        public void Increment(FoodChainRole role)
        {
            var count = GetCount(role) + 1;
            _countsByRole[role] = count;

            Changed?.Invoke(role, count);
        }
    }
}
