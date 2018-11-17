using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Shared
{
    internal class ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        #region References
        private Queue<T> objects;        
        private T prefab;
        private GameObject bag;
        #endregion

        #region Methods
        internal ObjectPool(T prefab)
        {
            this.prefab = prefab;
            objects = new Queue<T>();

            // create a gameObject to store all inactive instances.
            bag = new GameObject("-- ObjectPool: " + prefab.name);

            // keep the bag alive across scenes
            Object.DontDestroyOnLoad(bag);
        }
        
        public T Get()
        {
            return Get(Vector3.zero, Quaternion.identity, null);
        }

        public T Get(Vector3 position)
        {
            return Get(position, Quaternion.identity, null);
        }

        public T Get(Vector3 position, Transform parent)
        {
            return Get(position, Quaternion.identity, parent);
        }

        public T Get(Vector3 position, Quaternion rotation, Transform parent)
        {
            if (objects.Count == 0)
            {
                AddToPool(CreateInstance());
            }
            T instance = objects.Dequeue();

            // reset to prefab
            instance.transform.localScale = prefab.transform.localScale;
            instance.transform.localPosition = prefab.transform.localPosition;
            instance.transform.localRotation = prefab.transform.localRotation;

            instance.transform.parent = parent;
            if (parent != null)
            {
                instance.transform.localPosition = position;
            }
            else
            {
                instance.transform.position = position;
            }
            instance.transform.rotation = rotation;
            instance.gameObject.SetActive(true);
            
            return instance;
        }

        private T CreateInstance()
        {
            T instance = Object.Instantiate(prefab);

            instance.OnRemove += AddToPool;

            return instance;
        }

        private void AddToPool(T instance)
        {
            // Deactivate object.
            instance.gameObject.SetActive(false);

            // Add to bag for cleaner scene overview.
            instance.gameObject.transform.parent = bag.transform;
            
            objects.Enqueue(instance);
        }
        #endregion
    }
}