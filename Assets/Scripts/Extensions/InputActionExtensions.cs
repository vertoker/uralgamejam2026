using System;
using UniRx;
using UnityEngine.InputSystem;

namespace Extensions
{
    public static class InputActionExtensions
    {
        /// <summary>
        /// Подписывает handler на started и возвращает IDisposable для корректной отписки.
        /// </summary>
        public static IDisposable SubscribeStarted(this InputAction action, Action<InputAction.CallbackContext> handler)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            if (handler == null) 
                throw new ArgumentNullException(nameof(handler));

            action.started += handler;
            return Disposable.Create(() => action.started -= handler);
        }
        
        /// <summary>
        /// Подписывает handler на canceled и возвращает IDisposable для корректной отписки.
        /// </summary>
        public static IDisposable SubscribeCanceled(this InputAction action, Action<InputAction.CallbackContext> handler)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            if (handler == null) 
                throw new ArgumentNullException(nameof(handler));

            action.canceled += handler;
            return Disposable.Create(() => action.canceled -= handler);
        }
        
        /// <summary>
        /// Подписывает handler на performed и возвращает IDisposable для корректной отписки.
        /// </summary>
        public static IDisposable SubscribePerformed(this InputAction action, Action<InputAction.CallbackContext> handler)
        {
            if (action == null) 
                throw new ArgumentNullException(nameof(action));
            if (handler == null) 
                throw new ArgumentNullException(nameof(handler));

            action.performed += handler;
            return Disposable.Create(() => action.performed -= handler);
        }
    }
}