using Effects;
using Recognition;
using Runes;
using Services;
using Spells;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameWindow _gameWindowPrefab;
    [SerializeField] private InputActionAsset _inputActionAsset;
    
    [SerializeField] private EffectsSettings _effectsSettings;
    [SerializeField] private RecognitionSettings _recognitionSettings;
    [SerializeField] private SpellsSettings _spellsSettings;
    
    protected override void Configure(IContainerBuilder builder)
    {
        var gameWindow = Instantiate(_gameWindowPrefab);
        
        builder.RegisterInstance(_camera);
        builder.RegisterInstance(gameWindow);
        builder.RegisterInstance(_inputActionAsset);
        
        builder.RegisterInstance(_effectsSettings);
        builder.RegisterInstance(_recognitionSettings);
        builder.RegisterInstance(_spellsSettings);
        
        // Input
        builder.Register<CursorService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<InputProvider>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Rendering
        builder.Register<ZernikeRecognizer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<LineDrawerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<VFXDrawerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Gameplay
        builder.Register<RunesProvider>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<RuneColorMixer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<SymbolService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<SpellService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<CastService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        builder.Register<GameEntryPoint>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        Debug.Log($"{nameof(GameScope)} is configured");
    }
}
