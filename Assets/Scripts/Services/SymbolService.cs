using System;
using System.Collections.Generic;
using Effects;
using Recognition;
using Runes;
using UniRx;
using UnityEngine;
using VContainer.Unity;
using World;

namespace Services
{
    public class SymbolService : IInitializable, IDisposable
    {
        private readonly Camera _camera;
        private readonly EffectsSettings _settings;
        private readonly InputProvider _inputProvider;
        private readonly RunesProvider _runesProvider;
        private readonly GameModesService _gameModesService;
        private readonly ZernikeRecognizer _zernikeRecognizer;
        private readonly LineDrawerService _lineDrawerService;
        private readonly VFXDrawerService _vfxDrawerService;
        
        private readonly List<Vector2> _pointsCache;
        private DrawEffectPointVFX _templateComponent;
        private LineDrawer _lineDrawer;
        private bool _inDraw;
        
        private readonly CompositeDisposable _disposables = new();
        
        private readonly Subject<Unit> _onStartDraw = new();
        private readonly Subject<SymbolFeaturesScriptable> _onDraw = new();
        
        public IObservable<Unit> OnStartDraw => _onStartDraw;
        public IObservable<SymbolFeaturesScriptable> OnDraw => _onDraw;

        public SymbolService(Player player, EffectsSettings settings, InputProvider inputProvider, 
            ZernikeRecognizer zernikeRecognizer, RunesProvider runesProvider, GameModesService gameModesService,
            LineDrawerService lineDrawerService, VFXDrawerService vfxDrawerService)
        {
            _camera = player.Camera;
            _settings = settings;
            _inputProvider = inputProvider;
            _runesProvider = runesProvider;
            _gameModesService = gameModesService;
            _zernikeRecognizer = zernikeRecognizer;
            _lineDrawerService = lineDrawerService;
            _vfxDrawerService = vfxDrawerService;

            _pointsCache = new List<Vector2>(_settings.PointsCache);
        }

        public void Initialize()
        {
            _inputProvider.CursorActive.Subscribe(OnCursorActive).AddTo(_disposables);
            _inputProvider.CursorPosition.Subscribe(OnCursorUpdate).AddTo(_disposables);
            _runesProvider.RuneGroup.Subscribe(OnRuneGroupChanged).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        private void OnRuneGroupChanged(RuneGroup runeGroup)
        {
            if (_inDraw)
            {
                DrawEnd();
                DrawStart();
            }
        }
        private void OnCursorActive(bool active)
        {
            if (!active)
            {
                DrawEnd();
            }
            else if (_gameModesService.IsMagicMode.Value)
            {
                DrawStart();
            }
        }
        private void OnCursorUpdate(Vector2 position)
        {
            if (!_gameModesService.IsMagicMode.Value) return;
            
            if (_inputProvider.CursorActive.Value)
            {
                DrawUpdate(position);
            }
        }

        private void DrawStart()
        {
            _pointsCache.Clear();
            var color = _runesProvider.GetColor(_runesProvider.RuneGroup.Value);
            if (_settings.UseLine) _lineDrawer = _lineDrawerService.Create(color);
            if (_settings.UseVFX) _templateComponent = new DrawEffectPointVFX(Vector3.zero, color);
            _onStartDraw.OnNext(Unit.Default);
        }
        private void DrawEnd()
        {
            _inDraw = false;
            if (_settings.UseLine) _lineDrawerService.Return(_lineDrawer, _settings.LineLifetime);

            if (_pointsCache.Count != 0)
            {
                var symbolFeatures = _zernikeRecognizer.Recognize(_pointsCache);
                _onDraw.OnNext(symbolFeatures);
                _pointsCache.Clear();
            }
        }
        private void DrawUpdate(Vector2 position)
        {
            // Debug.Log($"Mouse position: {position}");
            if (_pointsCache.Count > 0 && _pointsCache[^1] == position) return;
                
            _inDraw = true;
            _pointsCache.Add(position);
            var ray = _camera.ScreenPointToRay(position);
            var point = ray.GetPoint(_settings.DrawingDistance);

            if (_settings.UseLine)
            {
                _lineDrawer.AddPosition(point);
            }
            if (_settings.UseVFX)
            {
                var component = _templateComponent;
                component.Position = point;
                _vfxDrawerService.AddRequest(component);
            }
        }
    }
}