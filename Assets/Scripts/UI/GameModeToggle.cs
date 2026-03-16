using Services;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI
{
    public class GameModeToggle : MonoBehaviour
    {
        [field: SerializeField] public bool ActiveOnMagicMode { get; private set; } = true;
        [field: SerializeField] public GameObject TipGO { get; private set; }
        
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(GameModesService gameModesService)
        {
            gameModesService.IsMagicMode.Subscribe(OnMagicModeChanged).AddTo(_disposables);
        }
        private void OnDestroy()
        {
            _disposables.Clear();
        }
        
        private void OnMagicModeChanged(bool value)
        {
            value = ActiveOnMagicMode ? value : !value;
            TipGO.SetActive(value);
        }
    }
}