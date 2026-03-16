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
        
        private readonly CompositeDisposable _disposables = new();
        
        private readonly Subject<Unit> _onStartDraw = new();
        private readonly Subject<SymbolFeaturesScriptable> _onDraw = new();
        
        public IObservable<Unit> OnStartDraw => _onStartDraw;
        public IObservable<SymbolFeaturesScriptable> OnDraw => _onDraw;

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
                _onStartDraw.OnNext(Unit.Default);
            }
            else
            {
                _lineDrawerService.Return(_testLineDrawer, 3f);
                
                if (_pointsCache.Count != 0)
                {
                    var symbolFeatures = _zernikeRecognizer.Recognize(_pointsCache);
                    _onDraw.OnNext(symbolFeatures);
                }
            }
        }
        private void OnCursorUpdate(Vector2 position)
        {
            if (_inputProvider.CursorActive.Value)
            {
                // Debug.Log($"Mouse position: {position}");
                if (_pointsCache.Count > 0 && _pointsCache[^1] == position) return;
                
                _pointsCache.Add(position);
                var ray = _camera.ScreenPointToRay(position);
                var point = ray.GetPoint(3f);
                _testLineDrawer.AddPosition(point);
            }
        }
    }
}