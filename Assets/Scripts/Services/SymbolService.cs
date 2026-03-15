using System;
using System.Collections.Generic;
using Effects;
using Recognition;
using Runes;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class SymbolService : IInitializable, IDisposable
    {
        private readonly Camera _camera;
        private readonly InputProvider _inputProvider;
        private readonly RunesProvider _runesProvider;
        private readonly RuneColorMixer _colorMixer;
        private readonly ZernikeRecognizer _zernikeRecognizer;
        private readonly LineDrawerService _lineDrawerService;
        
        private readonly Subject<SymbolFeaturesScriptable> _onSymbolDraw = new();
        public IObservable<SymbolFeaturesScriptable> OnSymbolDraw => _onSymbolDraw;
        
        private readonly CompositeDisposable _disposables = new();

        public SymbolService(Camera camera, InputProvider inputProvider, 
            RunesProvider runesProvider, RuneColorMixer colorMixer,
            ZernikeRecognizer zernikeRecognizer, LineDrawerService lineDrawerService)
        {
            _camera = camera;
            _inputProvider = inputProvider;
            _runesProvider = runesProvider;
            _colorMixer = colorMixer;
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
                var color = _colorMixer.GetColor(_runesProvider.RuneGroup.Value);
                _testLineDrawer = _lineDrawerService.Create(color);
            }
            else
            {
                _lineDrawerService.Return(_testLineDrawer, 3f);
                var symbolFeatures = _zernikeRecognizer.Recognize(_pointsCache);
                _onSymbolDraw.OnNext(symbolFeatures);
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