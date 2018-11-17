using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Shared
{
    public class ObjectPoolFactory
    {
        private IDictionary<IPoolable, IObjectPool> pools;
        private static ObjectPoolFactory instance;

        private static ObjectPoolFactory Instance
        {
            get
            {
                // create instance if it doesn't exist.
                if (instance == null)
                {
                    instance = new ObjectPoolFactory();
                }
                return instance;
            }
        }

        private ObjectPoolFactory()
        {
            pools = new Dictionary<IPoolable, IObjectPool>();
        }

        private IObjectPool<T> GetPool<T>(T prefab) where T : MonoBehaviour, IPoolable<T>
        {
            IObjectPool pool;
            if (!pools.TryGetValue(prefab, out pool))
            {
                pool = new ObjectPool<T>(prefab);
                pools.Add(prefab, pool);
            }
            return (IObjectPool<T>)pool;
        }

        public static IObjectPool<T> Get<T>(T prefab) where T : MonoBehaviour, IPoolable<T>
        {
            return Instance.GetPool<T>(prefab);
        }
    }
}
