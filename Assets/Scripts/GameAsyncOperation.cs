using System;

namespace Asteroids
{
    public class GameAsyncOperation
    {
        public event Action completed;

        public void Done()
        {
            completed?.Invoke();
        }
    }
}