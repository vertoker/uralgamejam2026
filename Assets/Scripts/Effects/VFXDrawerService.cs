using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Effects
{
    public class VFXDrawerService : ITickable
    {
        private readonly EffectsSettings _settings;
        private readonly VisualEffect _drawEffect;
        
        private readonly GraphicsBuffer _pointsBuffer;
        private readonly DrawEffectPointVFX[] _points;
        private int _pointsBufferCount;

        private static readonly int PositionBuffer = Shader.PropertyToID("PositionBuffer");
        private static readonly int PositionBufferCount = Shader.PropertyToID("PositionBufferCount");
        private static readonly int SpawnBatchEvent = Shader.PropertyToID("SpawnBatch");
        private static readonly int RemoveBatchEvent = Shader.PropertyToID("RemoveBatch");
        
        public VFXDrawerService(EffectsSettings settings)
        {
            _settings = settings;
            _drawEffect = Object.Instantiate(settings.PrefabDrawEffect);

            _pointsBufferCount = 0;
            _pointsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 
                _settings.MaxBufferDrawPoints, Marshal.SizeOf<DrawEffectPointVFX>());
            
            _points = new DrawEffectPointVFX[_settings.MaxBufferDrawPoints];
            _pointsBuffer.SetData(_points);
            
            _drawEffect.SetGraphicsBuffer(PositionBuffer, _pointsBuffer);
            _drawEffect.SetInt(PositionBufferCount, _pointsBufferCount);
        }

        public void Tick()
        {
            if (_pointsBufferCount != 0 && _pointsBuffer.IsValid())
            {
                _pointsBuffer.SetData(_points, 0, 0, _pointsBufferCount);
                _drawEffect.SetInt(PositionBufferCount, math.min(_pointsBufferCount, _settings.MaxBufferDrawPoints));
                _drawEffect.SendEvent(SpawnBatchEvent);
                _pointsBufferCount = 0;
            }
        }

        public void AddRequest(DrawEffectPointVFX component)
        {
            if (!_settings.UseVFX) return;
            
            // Debug.Log($"Count: {_pointsBufferCount}, Limit: {_settings.MaxBufferDrawPoints}");
            if (_pointsBufferCount < _pointsBuffer.count)
            {
                _points[_pointsBufferCount] = component;
                _pointsBufferCount++;
            }
        }
        public void ClearRequests()
        {
            if (!_settings.UseVFX) return;
            
            _drawEffect.SendEvent(RemoveBatchEvent);
        }
    }
}