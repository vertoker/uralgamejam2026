using UI;
using VContainer;
using VContainer.Unity;

public class GameEntryPoint : IStartable
{
    private readonly GameWindow _gameWindow;
    private readonly LifetimeScope _scope;

    public GameEntryPoint(GameWindow gameWindow, LifetimeScope scope)
    {
        _gameWindow = gameWindow;
        _scope = scope;
    }

    public void Start()
    {
        _scope.Container.InjectGameObject(_gameWindow.gameObject);
    }
}