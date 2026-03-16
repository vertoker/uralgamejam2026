using Effects;
using Recognition;
using Services;
using Spells;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using World;

public class GameScope : LifetimeScope
{
    [SerializeField] private Player _player;
    [SerializeField] private InputActionAsset _inputActionAsset;
    [SerializeField] private GameWindow _gameWindowPrefab;
    [SerializeField] private PauseWindow _pauseWindowPrefab;
    
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private EffectsSettings _effectsSettings;
    [SerializeField] private RecognitionSettings _recognitionSettings;
    [SerializeField] private SpellsSettings _spellsSettings;
    
    protected override void Configure(IContainerBuilder builder)
    {
        var gameWindow = Instantiate(_gameWindowPrefab);
        var pauseWindow = Instantiate(_pauseWindowPrefab);
        pauseWindow.gameObject.SetActive(false);
        
        builder.RegisterInstance(gameWindow);
        builder.RegisterInstance(pauseWindow);
        builder.RegisterInstance(_player);
        builder.RegisterInstance(_inputActionAsset);
        
        builder.RegisterInstance(_gameSettings);
        builder.RegisterInstance(_effectsSettings);
        builder.RegisterInstance(_recognitionSettings);
        builder.RegisterInstance(_spellsSettings);
        
        // Input
        builder.Register<CursorService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<InputProvider>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Rendering
        builder.Register<LineDrawerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<VFXDrawerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Core Game
        builder.Register<PlayerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<GameModesService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<GameUIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Magic Systems
        builder.Register<ZernikeRecognizer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<RunesProvider>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<SymbolService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<SpellService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<CastService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Entry
        builder.Register<GameEntryPoint>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        Debug.Log($"{nameof(GameScope)} is configured");
    }
}
