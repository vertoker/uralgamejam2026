using System;
using UnityEngine;

namespace Recognition
{
    [Serializable]
    public struct SymbolFeatures
    {
        [field: SerializeField] public float[] ZernikeMoments { get; private set; }// = Array.Empty<float>();
        [field: SerializeField] public float[] AngularHistogram { get; private set; }// = Array.Empty<float>();
        
        public SymbolFeatures(float[] zernikeMoments, float[] angularHistogram)
        {
            ZernikeMoments = zernikeMoments; // Моменты Цернике (default 4)
            AngularHistogram = angularHistogram; // Угловая гистограмма (default 8)
        }
    }
}