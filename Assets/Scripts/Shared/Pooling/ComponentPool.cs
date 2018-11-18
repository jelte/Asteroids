using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Shared
{
    /**
     * Allows for reusing components.
     **/
    public class ComponentPool<T> where T : Behaviour
    {
        #region References
        private GameObject gameObject;
        private Queue<T> components;
        #endregion

        #region methods
        internal ComponentPool(GameObject gameObject)
        {
            this.gameObject = gameObject;
            components = new Queue<T>();
        }

        /**
         * get an instance from the queue or create a new one.
         **/
        public T Get()
        {
            if (components.Count == 0)
            {
                // create a new component
                return CreateComponent();
            }
            // get an instance from the queue
            T component = components.Dequeue();

            // enable the component
            component.enabled = true;

            return component;
        }

        /**
         * Add an instance back to the queue.
         **/
        public void Add(T component)
        {
            // disable the component
            component.enabled = false;

            components.Enqueue(component);
        }
        
        private T CreateComponent()
        {
            return gameObject.AddComponent<T>();
        }
        #endregion
    }
}
