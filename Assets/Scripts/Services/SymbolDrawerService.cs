using System;
using System.Collections.Generic;
using Effects;
using Recognition;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class SymbolDrawerService : IInitializable, IDisposable
    {
        private readonly Camera _camera;
        private readonly InputProvider _inputProvider;
        private readonly ZernikeRecognizer _zernikeRecognizer;
        private readonly LineDrawerService _lineDrawerService;
        private readonly CompositeDisposable _disposables = new();

        public SymbolDrawerService(Camera camera, InputProvider inputProvider,
            ZernikeRecognizer zernikeRecognizer, LineDrawerService lineDrawerService)
        {
            _camera = camera;
            _inputProvider = inputProvider;
            _zernikeRecognizer = zernikeRecognizer;
            _lineDrawerService = lineDrawerService;
        }

        public void Initialize()
        {
            _inputProvider.CursorActive.Subscribe(OnCursorActive).AddTo(_disposables);
            _inputProvider.CursorPosition.Subscribe(OnCursorUpdate).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        private readonly List<Vector2> _pointsCache = new(512);
        private LineDrawer _testLineDrawer;
        
        private void OnCursorActive(bool active)
        {
            if (active)
            {
                _pointsCache.Clear();
                _testLineDrawer = _lineDrawerService.Create();
            }
            else
            {
                _zernikeRecognizer.Recognize(_pointsCache);
                _lineDrawerService.Return(_testLineDrawer, 3f);
            }
        }
        private void OnCursorUpdate(Vector2 position)
        {
            if (_inputProvider.CursorActive.Value)
            {
                // Debug.Log($"Mouse position: {position}");
                _pointsCache.Add(position);
                var ray = _camera.ScreenPointToRay(position);
                var point = ray.GetPoint(3f);
                _testLineDrawer.AddPosition(point);
            }
        }
    }
}