using System;
using UnityEngine;

namespace Recognition
{
    [CreateAssetMenu(menuName = "Recognition/" + nameof(RecognitionSymbol), 
        fileName = nameof(RecognitionSymbol))]
    public class RecognitionSymbol : ScriptableObject
    {
        [field: SerializeField] public float[] ZernikeMoments { get; private set; } = Array.Empty<float>();
        [field: SerializeField] public float[] AngularHistogram { get; private set; } = Array.Empty<float>();
    }
}