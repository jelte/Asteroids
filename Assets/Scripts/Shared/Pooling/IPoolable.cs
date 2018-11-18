using System;

namespace Asteroids.Shared.Pooling
{
    public interface IPoolable { }

    public interface IPoolable<T> : IPoolable
    {
        event Action<T> OnRemove;
    }
}