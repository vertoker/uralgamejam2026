using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    [CreateAssetMenu(menuName = "Spells/" + nameof(SpellsSettings), 
        fileName = nameof(SpellsSettings))]
    public class SpellsSettings : ScriptableObject
    {
        [field: SerializeField] public float ActivateTime { get; private set; } = 1f;
        
        [SerializeField] private SpellScriptable[] _spells = Array.Empty<SpellScriptable>();
        public IReadOnlyList<SpellScriptable> Spells => _spells;
    }
}