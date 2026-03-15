using Effects;
using Recognition;
using Services;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameWindow _gameWindowPrefab;
    [SerializeField] private EffectsSettings _effectsSettings;
    [SerializeField] private RecognitionData _recognitionData;
    [SerializeField] private InputActionAsset _inputActionAsset;
    
    protected override void Configure(IContainerBuilder builder)
    {
        var gameWindow = Instantiate(_gameWindowPrefab);
        
        builder.RegisterInstance(_camera);
        builder.RegisterInstance(gameWindow);
        builder.RegisterInstance(_effectsSettings);
        builder.RegisterInstance(_recognitionData);
        builder.RegisterInstance(_inputActionAsset);
        
        // Input
        builder.Register<CursorService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<InputProvider>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Rendering
        builder.Register<LineDrawerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        // Gameplay
        builder.Register<ZernikeRecognizer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<SymbolDrawerService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        Debug.Log($"{nameof(GameScope)} is configured");
    }
}
