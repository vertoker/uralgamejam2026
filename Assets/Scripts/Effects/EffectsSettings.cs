using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(menuName = "Effects/" + nameof(EffectsSettings), 
        fileName = nameof(EffectsSettings))]
    public class EffectsSettings : ScriptableObject
    {
        [field: SerializeField] public LineRenderer PrefabRenderer { get; private set; }
    }
}