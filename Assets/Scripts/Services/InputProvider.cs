using System;
using System.Collections.Generic;
using Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Services
{
    public class InputProvider : IInitializable, IDisposable
    {
        public ReactiveProperty<Vector2> MoveDirection { get; } = new(Vector2.zero);
        public ReactiveProperty<Vector2> LookDirection { get; } = new(Vector2.zero);
        public ReactiveProperty<Vector2> CursorPosition { get; } = new(Vector2.zero);
        
        public ReactiveProperty<bool> Cancel { get; } = new(false);
        public ReactiveProperty<bool> MagicMode { get; } = new(false);
        public ReactiveProperty<bool> CursorActive { get; } = new(false);

        private readonly ReactiveProperty<bool>[] _modifiers = new ReactiveProperty<bool>[ModifiersCount];
        public IReadOnlyList<IReadOnlyReactiveProperty<bool>> Modifiers => _modifiers;
        
        public const int ModifiersCount = 6;
        
        private readonly CursorService _cursorService;
        
        private InputActionMap _playerMap;
        private InputActionMap _uiMap;
        
        private InputAction _move;
        private InputAction _look;
        private InputAction _cursorPosition;
        
        private InputAction _cancel;
        private InputAction _magicMode;
        private InputAction _cursorActive;
        
        private readonly InputAction[] _modifierActions = new InputAction[ModifiersCount];

        private readonly CompositeDisposable _disposables = new();

        public InputProvider(CursorService cursorService, InputActionAsset inputActionAsset)
        {
            _cursorService = cursorService;
            for (var i = 0; i < ModifiersCount; i++)
                _modifiers[i] = new ReactiveProperty<bool>(false);
            
            FindActions(inputActionAsset);
        }
        public void Initialize()
        {
            EnableActions();
            Subscribe();
        }
        public void Dispose()
        {
            Unsubscribe();
            DisableActions();
        }
        
        private void FindActions(InputActionAsset inputActionAsset)
        {
            _playerMap = inputActionAsset.FindActionMap("Player", true);
            _uiMap = inputActionAsset.FindActionMap("UI", true);

            _move = _playerMap.FindAction("Move");
            _look = _playerMap.FindAction("Look");
            _cursorPosition = _playerMap.FindAction("CursorPosition");
            
            _cancel = _playerMap.FindAction("Cancel");
            _magicMode = _playerMap.FindAction("MagicMode");
            _cursorActive = _playerMap.FindAction("CursorActive");

            for (var i = 0; i < ModifiersCount; i++)
                _modifierActions[i] = _playerMap.FindAction($"Modifier {i+1}");
        }

        private void EnableActions()
        {
            _playerMap.Enable();
            _uiMap.Enable();
        }
        private void DisableActions()
        {
            _playerMap.Disable();
            _uiMap.Disable();
        }
        
        private void Subscribe()
        {
            _move.SubscribePerformed(MoveOnPerformed).AddTo(_disposables);
            _move.SubscribeCanceled(MoveOnCancelled).AddTo(_disposables);
            _look.SubscribePerformed(LookOnPerformed).AddTo(_disposables);
            _look.SubscribeCanceled(LookOnCancelled).AddTo(_disposables);
            _cursorPosition.SubscribePerformed(CursorPositionOnPerformed).AddTo(_disposables);
            _cursorPosition.SubscribeCanceled(CursorPositionOnCancelled).AddTo(_disposables);
            
            _cancel.SubscribePerformed(CancelOnPerformed).AddTo(_disposables);
            _cancel.SubscribeCanceled(CancelOnCancelled).AddTo(_disposables);
            _magicMode.SubscribePerformed(MagicModeOnPerformed).AddTo(_disposables);
            _magicMode.SubscribeCanceled(MagicModeOnCancelled).AddTo(_disposables);
            _cursorActive.SubscribePerformed(CursorActiveOnPerformed).AddTo(_disposables);
            _cursorActive.SubscribeCanceled(CursorActiveOnCancelled).AddTo(_disposables);

            for (var i = 0; i < ModifiersCount; i++)
            {
                var iCached = i;
                _modifierActions[i].SubscribePerformed(ctx => ModifiersOnPerformed(ctx, iCached)).AddTo(_disposables);
                _modifierActions[i].SubscribeCanceled(ctx => ModifiersOnCancelled(ctx, iCached)).AddTo(_disposables);
            }
        }
        private void Unsubscribe()
        {
            _disposables.Clear();
        }

        private void MoveOnPerformed(InputAction.CallbackContext ctx) => MoveDirection.Value = ctx.ReadValue<Vector2>();
        private void MoveOnCancelled(InputAction.CallbackContext ctx) => MoveDirection.Value = Vector2.zero;
        private void LookOnPerformed(InputAction.CallbackContext ctx) => LookDirection.Value = ctx.ReadValue<Vector2>();
        private void LookOnCancelled(InputAction.CallbackContext ctx) => LookDirection.Value = Vector2.zero;
        private void CursorPositionOnPerformed(InputAction.CallbackContext ctx) => CursorPosition.Value = ctx.ReadValue<Vector2>();
        private void CursorPositionOnCancelled(InputAction.CallbackContext ctx) => CursorPosition.Value = Vector2.zero;
        
        private void CancelOnPerformed(InputAction.CallbackContext ctx) => Cancel.Value = true;
        private void CancelOnCancelled(InputAction.CallbackContext ctx) => Cancel.Value = false;
        private void MagicModeOnPerformed(InputAction.CallbackContext ctx) => MagicMode.Value = true;
        private void MagicModeOnCancelled(InputAction.CallbackContext ctx) => MagicMode.Value = false;
        private void CursorActiveOnPerformed(InputAction.CallbackContext ctx) => CursorActive.Value = true;
        private void CursorActiveOnCancelled(InputAction.CallbackContext ctx) => CursorActive.Value = false;
        
        private void ModifiersOnPerformed(InputAction.CallbackContext ctx, int index) => _modifiers[index].Value = true;
        private void ModifiersOnCancelled(InputAction.CallbackContext ctx, int index) => _modifiers[index].Value = false;
    }
}