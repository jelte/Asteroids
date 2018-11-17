using System;

namespace Asteroids.Shared
{
    public interface IPoolable { }

    public interface IPoolable<T> : IPoolable
    {
        event Action<T> OnRemove;
    }
}