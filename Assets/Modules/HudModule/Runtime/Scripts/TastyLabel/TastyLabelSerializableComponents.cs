using UnityEngine;

namespace Modules.HudModule.Runtime.Scripts.TastyLabel
{
    public class TastyLabelSerializableComponents : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
    }
}
