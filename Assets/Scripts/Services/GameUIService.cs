using System;
using UI;
using VContainer.Unity;

namespace Services
{
    public class GameUIService : IInitializable, IDisposable
    {
        private readonly GameWindow _gameWindow;

        public GameUIService(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
        }

        public void Initialize()
        {
            
        }
        public void Dispose()
        {
            
        }
    }
}