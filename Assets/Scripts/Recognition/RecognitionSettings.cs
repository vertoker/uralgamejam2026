using System;
using UnityEngine;

namespace Recognition
{
    [CreateAssetMenu(menuName = "Recognition/" + nameof(RecognitionSettings), 
        fileName = nameof(RecognitionSettings))]
    public class RecognitionSettings : ScriptableObject
    {
        [field: Range(0f, 1f), SerializeField]
        public float RecognitionThreshold { get; private set; } = 0.7f;
        
        [field: Range(0f, 1f), SerializeField]
        public float ZernikeWeight { get; private set; } = 0.7f;
        
        [field: Min(1), SerializeField]
        public int ZernikeOrder { get; private set; } = 4;
        
        [field: Min(1), SerializeField]
        public int AngularCount { get; private set; } = 12;
        
        [field: Min(1), SerializeField]
        public int ResamplePoints { get; private set; } = 64;
        
        [field: SerializeField]
        public bool CreateAssetOnRecognition { get; private set; } = false;
        
        [field: SerializeField]
        public SymbolFeaturesScriptable[] Templates { get; private set; } = Array.Empty<SymbolFeaturesScriptable>();
    }
}