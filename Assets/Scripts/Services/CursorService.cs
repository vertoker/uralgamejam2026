using System;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class CursorService : IInitializable, IDisposable
    {
        public CursorLockMode Mode { get; private set; } = CursorLockMode.None;
        public bool CurrentFocus { get; private set; }

        public void Initialize()
        {
            Application.focusChanged += ApplicationOnFocus;
            ApplicationOnFocus(Application.isFocused);
        }
        public void Dispose()
        {
            Application.focusChanged -= ApplicationOnFocus;
        }

        private void ApplicationOnFocus(bool focus)
        {
            CurrentFocus = focus;
            UpdateCursor();
        }

        /// <summary> Без ограничений (visible = true, lockState = None) </summary>
        public void SetNone() => SetMode(CursorLockMode.None);

        /// <summary> Ограниченная по краям (visible = true, lockState = Confined) </summary>
        public void SetClamped() => SetMode(CursorLockMode.Confined);

        /// <summary> Скрытая/залоченная мышь в центре (visible = false, lockState = Locked) </summary>
        public void SetLocked() => SetMode(CursorLockMode.Locked);

        public void SetMode(CursorLockMode mode)
        {
            Mode = mode;
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            // ReSharper disable once SimplifyConditionalTernaryExpression
            Cursor.visible = CurrentFocus ? Mode != CursorLockMode.Locked : false;
            Cursor.lockState = CurrentFocus ? Mode : CursorLockMode.None;

            //Debug.Log($"Mode: {Mode}, CurrentFocus: {CurrentFocus}, hash code: {GetHashCode()}");
            //Debug.Log($"Visible: {UnityEngine.Cursor.visible}, Locked: {UnityEngine.Cursor.lockState}");
        }
    }
}