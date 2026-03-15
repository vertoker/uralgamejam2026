using UnityEngine;
using Object = UnityEngine.Object;

namespace Effects
{
    public struct LineDrawer
    {
        private readonly LineRenderer _renderer;
        private int _counter;
        
        public LineDrawer(LineRenderer prefabRenderer, Color color)
        {
            _renderer = Object.Instantiate(prefabRenderer);
            _counter = _renderer.positionCount = 0;
            _renderer.startColor = _renderer.endColor = color;
        }

        public void AddPosition(Vector3 position)
        {
            var nextCounter = _counter + 1;
            _renderer.positionCount = nextCounter;
            _renderer.SetPosition(_counter, position);
            _counter = nextCounter;
        }
        
        public void Destroy()
        {
            if (!_renderer) return;
            Object.Destroy(_renderer.gameObject);
        }
        public void DestroyAsync(float delay)
        {
            if (!_renderer) return;
            Object.Destroy(_renderer.gameObject, delay);
        }
    }
}