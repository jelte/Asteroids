using System;

namespace Asteroids.Shared
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