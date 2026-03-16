using UniRx;

namespace Services
{
    public class GameUIService
    {
        private readonly InputProvider _inputProvider;
        private readonly GameModesService _gameModesService;

        private readonly CompositeDisposable _disposables = new();
        
        public GameUIService(InputProvider inputProvider, GameModesService gameModesService)
        {
            _inputProvider = inputProvider;
            _gameModesService = gameModesService;
        }
        public void Initialize()
        {
            _inputProvider.Cancel.Subscribe(OnCancel).AddTo(_disposables);
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnCancel(bool value)
        {
            if (!value) return;
            
            // TODO open pause window
        }
    }
}