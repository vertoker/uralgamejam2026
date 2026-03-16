using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;
using World;

namespace Services
{
    public class PlayerService : IInitializable, ITickable, IDisposable
    {
        private readonly Player _player;
        private readonly GameSettings _gameSettings;
        private readonly GameModesService _gameModesService;
        private readonly InputProvider _inputProvider;

        private readonly CompositeDisposable _disposables = new();

        public PlayerService(Player player, GameSettings gameSettings,
            GameModesService gameModesService, InputProvider inputProvider)
        {
            _player = player;
            _gameSettings = gameSettings;
            _gameModesService = gameModesService;
            _inputProvider = inputProvider;
        }

        public void Initialize()
        {
            _inputProvider.LookDirection.Subscribe(OnLook).AddTo(_disposables);
            // _inputProvider.MoveDirection.Subscribe(OnMove).AddTo(_disposables);
        }
        public void Tick()
        {
            // OnLook(_inputProvider.LookDirection.Value);
            OnMove(_inputProvider.MoveDirection.Value);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
        
        private Vector2 _axis;
        private void OnLook(Vector2 direction)
        {
            if (_gameModesService.IsMagicMode.Value) return;
            if (direction == Vector2.zero) return;
            
            const float userSensitivity = 1f;
            _axis.y += direction.x * _gameSettings.MouseSensitivityX * userSensitivity * Time.deltaTime;
            _axis.x -= direction.y * _gameSettings.MouseSensitivityY * userSensitivity * Time.deltaTime;
            _axis.x = Mathf.Clamp(_axis.x, _gameSettings.MouseYMin, _gameSettings.MouseYMax);

            var rotation = _player.CameraTransform.rotation;
            rotation = Quaternion.Lerp(rotation, Quaternion.Euler(_axis.x, _axis.y, 0f), _gameSettings.CameraLerp);
            _player.CameraTransform.rotation = rotation;
        }
        private void OnMove(Vector2 direction)
        {
            if (_gameModesService.IsMagicMode.Value) return;
            
            var motion2D = direction.normalized * _gameSettings.PlayerSpeed * Time.deltaTime;
            var motion = GetMoveDirection(motion2D);
            _player.CharacterController.Move(motion);
        }
        
        private Vector3 GetMoveDirection(Vector2 direction)
        {
            var cameraForward = _player.CameraTransform.forward;
            var cameraRight = _player.CameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            return cameraForward * direction.y + cameraRight * direction.x;
        }
    }
}