using System;
using UI;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class GameUIService : IInitializable, IDisposable
    {
        private readonly InputProvider _inputProvider;
        private readonly CursorService _cursorService;
        private readonly GameModesService _gameModesService;
        private readonly PauseWindow _pauseWindow;

        private bool _isPlay = true;
        
        private readonly CompositeDisposable _disposables = new();
        
        public GameUIService(InputProvider inputProvider, CursorService cursorService,
            GameModesService gameModesService, PauseWindow pauseWindow)
        {
            _inputProvider = inputProvider;
            _cursorService = cursorService;
            _gameModesService = gameModesService;
            _pauseWindow = pauseWindow;
        }
        public void Initialize()
        {
            _inputProvider.Cancel.Subscribe(OnCancel).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnCancel(bool value)
        {
            if (!value) return;
            SetPlay(!_isPlay);
        }

        public void SetPlay(bool isPlay)
        {
            _isPlay = isPlay;

            if (_isPlay)
            {
                _pauseWindow.gameObject.SetActive(false);
                Time.timeScale = 1f;
                _gameModesService.ForceUpdateMagicMode();
            }
            else
            {
                _pauseWindow.gameObject.SetActive(true);
                Time.timeScale = 0f;
                _cursorService.SetNone();
            }
        }
    }
}