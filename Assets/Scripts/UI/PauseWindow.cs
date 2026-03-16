using Services;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class PauseWindow : MonoBehaviour
    {
        [field: SerializeField] public Button ContinueButton { get; private set; }
        [field: SerializeField] public Button ExitToMenuButton { get; private set; }

        private readonly CompositeDisposable _disposables = new();
        private GameUIService _gameUIService;

        [Inject]
        private void Construct(GameUIService gameUIService)
        {
            _gameUIService = gameUIService;
            ContinueButton.OnClickAsObservable().Subscribe(Continue).AddTo(_disposables);
            ExitToMenuButton.OnClickAsObservable().Subscribe(ExitToMenu).AddTo(_disposables);
        }
        private void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void Continue(Unit unit)
        {
            _gameUIService.SetPlay(true);
        }
        private void ExitToMenu(Unit unit)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}