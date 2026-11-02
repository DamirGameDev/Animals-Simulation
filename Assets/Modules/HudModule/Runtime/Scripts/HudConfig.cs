using Modules.SharedModule.Runtime.Scripts.Configs;
using UnityEngine;

namespace Modules.HudModule.Runtime.Scripts
{
    [ConfigScope(nameof(HudConfig))]
    [CreateAssetMenu(fileName = nameof(HudConfig), menuName = "Configs/Hud/" + nameof(HudConfig))]
    public class HudConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public float TastyLabelDuration { get; private set; } = 1;

        [field: Tooltip("Offset from the predator on the screen in pixels; negative y puts the label under it")]
        [field: SerializeField] public Vector2 TastyLabelScreenOffset { get; private set; } = new Vector2(0, -40);
    }
}
