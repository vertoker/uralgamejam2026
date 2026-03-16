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
        
        private readonly CompositeDisposable _disposables = new();
        private GameModesService _gameModesService;

        [Inject]
        private void Construct(RunesProvider runesProvider, GameModesService gameModesService)
        {
            _gameModesService = gameModesService;
            runesProvider.RuneGroup.Subscribe(OnGroupUpdated).AddTo(_disposables);
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
            value = _gameModesService.IsMagicMode.Value && value;
            
            ImageOn.gameObject.SetActive(value);
            ImageOff.gameObject.SetActive(!value);
        }
    }
}