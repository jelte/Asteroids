using Asteroids.Game;
using UnityEngine;

namespace Asteroids.Shared
{
    public interface IObjectPool { }

    public interface IObjectPool<T>  : IObjectPool where T : MonoBehaviour, IPoolable<T>
    {
        T Get();
        T Get(Vector3 position);
        T Get(Vector3 position, Transform parent);
        T Get(Vector3 position, Quaternion identity, Transform parent);
    }
}