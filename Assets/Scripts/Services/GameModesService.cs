using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class GameModesService : IInitializable, IDisposable
    {
        private readonly InputProvider _inputProvider;
        private readonly CursorService _cursorService;

        private readonly CompositeDisposable _disposables = new();
        
        private readonly ReactiveProperty<bool> _isMagicMode = new();
        public IReadOnlyReactiveProperty<bool> IsMagicMode => _isMagicMode;
        
        public GameModesService(InputProvider inputProvider, 
            CursorService cursorService, GameSettings gameSettings)
        {
            _inputProvider = inputProvider;
            _cursorService = cursorService;
            UpdateMagicMode(gameSettings.StartWithMagicMode);
        }
        public void Initialize()
        {
            _inputProvider.MagicMode.Subscribe(OnMagicModeChanged).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnMagicModeChanged(bool value)
        {
            if (!value) return;
            UpdateMagicMode(!_isMagicMode.Value);
        }
        private void UpdateMagicMode(bool value)
        {
            _cursorService.SetMode(value ? CursorLockMode.None : CursorLockMode.Locked);
            _isMagicMode.Value = value;
        }
    }
}