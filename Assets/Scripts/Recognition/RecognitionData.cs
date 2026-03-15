using System;
using UnityEngine;

namespace Recognition
{
    [CreateAssetMenu(menuName = "Recognition/" + nameof(RecognitionData), 
        fileName = nameof(RecognitionData))]
    public class RecognitionData : ScriptableObject
    {
        [field: SerializeField] public RecognitionSettings Settings { get; private set; }
        [field: SerializeField] public RecognitionSymbol[] Symbols { get; private set; } = Array.Empty<RecognitionSymbol>();
    }
}