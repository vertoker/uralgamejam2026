using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;

namespace Effects
{
    [StructLayout(LayoutKind.Sequential)] // for HLSL compatibility
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)] // only for graphics buffer
    public struct DrawEffectPointVFX
    {
        public Vector3 Position;
        public Color Color;

        public DrawEffectPointVFX(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}