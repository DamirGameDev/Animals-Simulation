using Modules.HudModule.Runtime.Scripts.DeathCounters;
using Modules.HudModule.Runtime.Scripts.TastyLabel;
using UnityEngine;

namespace Modules.HudModule.Runtime.Scripts
{
    public class HudSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public Canvas Canvas { get; private set; }
        [field: SerializeField] public DeathCountersSerializableComponents DeathCounters { get; private set; }
        [field: SerializeField] public RectTransform TastyLabelsRoot { get; private set; }
        [field: SerializeField] public TastyLabelSerializableComponents TastyLabelPrefab { get; private set; }
    }
}
