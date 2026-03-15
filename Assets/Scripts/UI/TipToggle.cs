using Services;
using UniRx;
using UnityEngine;
using VContainer;

namespace UI
{
    public class TipToggle : MonoBehaviour
    {
        [field: SerializeField] public GameObject TipGO { get; private set; }
        
        private readonly CompositeDisposable _disposables = new();
        private InputProvider _inputProvider;

        [Inject]
        private void Construct(InputProvider inputProvider)
        {
            _inputProvider = inputProvider;
            _inputProvider.Tip.Subscribe(OnTipChanged).AddTo(_disposables);
        }
        private void OnEnable()
        {
            TipGO.SetActive(false);
        }
        private void OnDestroy()
        {
            _disposables.Clear();
        }
        
        private void OnTipChanged(bool value)
        {
            TipGO.SetActive(value);
        }
    }
}