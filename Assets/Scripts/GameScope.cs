using Services;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class GameScope : LifetimeScope
{
    [SerializeField] private GameWindow _gameWindowPrefab;
    [SerializeField] private InputActionAsset _inputActionAsset;
    
    protected override void Configure(IContainerBuilder builder)
    {
        var gameWindow = Instantiate(_gameWindowPrefab);
        
        builder.RegisterInstance(gameWindow);
        builder.RegisterInstance(_inputActionAsset);
        
        builder.Register<CursorService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<InputProvider>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        builder.Register<GameUIService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        Debug.Log($"{nameof(GameScope)} is configured");
    }
}
