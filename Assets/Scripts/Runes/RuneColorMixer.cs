using UnityEngine;

namespace Runes
{
    public class RuneColorMixer
    {
        public Color GetColor(RuneGroup group)
        {
            const float mult1 = 1f / 3f; // 0.(3)
            const float mult2 = 1f - mult1; // 0.(6)7
                
            var alpha = group.Contains(RuneType.Alpha);
            var beta = group.Contains(RuneType.Beta);
            var gamma = group.Contains(RuneType.Gamma);
            var delta = group.Contains(RuneType.Delta);
            var epsilon = group.Contains(RuneType.Epsilon);
            var zeta = group.Contains(RuneType.Zeta);

            var r = 1f - (alpha ? mult1 : 0f) - (delta ? mult2 : 0f);
            var g = 1f - (beta ? mult1 : 0f) - (epsilon ? mult2 : 0f);
            var b = 1f - (gamma ? mult1 : 0f) - (zeta ? mult2 : 0f);
            
            return new Color(r, g, b);
        }
    }
}