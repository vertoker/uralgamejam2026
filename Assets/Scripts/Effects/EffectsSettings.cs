using UnityEngine;
using UnityEngine.VFX;

namespace Effects
{
    [CreateAssetMenu(menuName = "Effects/" + nameof(EffectsSettings), 
        fileName = nameof(EffectsSettings))]
    public class EffectsSettings : ScriptableObject
    {
        [field: Header("VFX")]
        [field: SerializeField] public bool UseVFX { get; private set; } = true;
        [field: SerializeField] public int MaxBufferDrawPoints { get; private set; } = 5000;
        [field: SerializeField] public VisualEffect PrefabDrawEffect { get; private set; }
        
        [field: Header("LineRenderer")]
        [field: SerializeField] public bool UseLine { get; private set; } = true;
        [field: SerializeField] public LineRenderer PrefabRenderer { get; private set; }
    }
}