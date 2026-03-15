using System;
using Runes;
using Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI
{
    public class RuneIconStatus : MonoBehaviour
    {
        [field: SerializeField] public RuneType RuneType { get; private set; }
        
        [field: SerializeField] public Image ImageOff { get; private set; }
        [field: SerializeField] public Image ImageOn { get; private set; }
        [field: SerializeField] public TMP_Text Text { get; private set; }
        [field: SerializeField] public TMP_Text TipText { get; private set; }
        
        [Inject] private RunesProvider _runesProvider;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(RunesProvider runesProvider)
        {
            _runesProvider = runesProvider;
            _runesProvider.RuneGroup.Subscribe(OnGroupUpdated).AddTo(_disposables);
        }
        private void OnEnable()
        {
            SetActive(false);
        }
        private void OnDestroy()
        {
            _disposables.Clear();
        }

        private void OnGroupUpdated(RuneGroup group)
        {
            var value = group.Contains(RuneType);
            SetActive(value);
        }
        public void SetActive(bool value)
        {
            ImageOn.gameObject.SetActive(value);
            ImageOff.gameObject.SetActive(!value);
        }
    }
}