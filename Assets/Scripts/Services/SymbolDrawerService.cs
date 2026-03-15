using System;
using System.Collections.Generic;
using Recognition;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class SymbolDrawerService : IInitializable, IDisposable
    {
        private readonly InputProvider _inputProvider;
        private readonly ZernikeRecognizer _zernikeRecognizer;
        private readonly CompositeDisposable _disposables = new();

        public SymbolDrawerService(InputProvider inputProvider, ZernikeRecognizer zernikeRecognizer)
        {
            _inputProvider = inputProvider;
            _zernikeRecognizer = zernikeRecognizer;
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
        private void OnCursorActive(bool active)
        {
            if (active)
            {
                _pointsCache.Clear();
            }
            else
            {
                _zernikeRecognizer.Recognize(_pointsCache);
            }
        }
        private void OnCursorUpdate(Vector2 position)
        {
            if (_inputProvider.CursorActive.Value)
            {
                Debug.Log($"Mouse position: {position}");
                _pointsCache.Add(position);
            }
        }
    }
}