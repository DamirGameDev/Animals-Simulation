using TMPro;
using UnityEngine;

namespace Modules.HudModule.Runtime.Scripts.DeathCounters
{
    public class DeathCountersSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text DeadPreysText { get; private set; }
        [field: SerializeField] public TMP_Text DeadPredatorsText { get; private set; }
    }
}
