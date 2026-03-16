using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    [CreateAssetMenu(menuName = "Spells/" + nameof(SpellScriptable), 
        fileName = nameof(SpellScriptable))]
    public class SpellScriptable : ScriptableObject
    {
        [SerializeField] private SpellStep[] _steps = Array.Empty<SpellStep>();
        public IReadOnlyList<SpellStep> Steps => _steps;
        
        [field: SerializeField] public GameObject PrefabEffect { get; private set; }
        [field: SerializeField] public float EffectLifetime { get; private set; } = 3f;
        [field: SerializeField] public float SpawnEffectDistance { get; private set; } = 3f;
    }
}