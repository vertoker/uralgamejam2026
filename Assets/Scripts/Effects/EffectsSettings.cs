using UnityEngine;
using UnityEngine.VFX;

namespace Effects
{
    [CreateAssetMenu(menuName = "Effects/" + nameof(EffectsSettings), 
        fileName = nameof(EffectsSettings))]
    public class EffectsSettings : ScriptableObject
    {
        [field: Header("VFX")]
        [field: SerializeField] public VisualEffect PrefabLineEffect { get; private set; }
        
        [field: Header("LineRenderer")]
        [field: SerializeField] public LineRenderer PrefabRenderer { get; private set; }
    }
}