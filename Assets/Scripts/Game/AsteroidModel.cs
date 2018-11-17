using Asteroids.Shared;
using System;
using UnityEngine;

namespace Asteroids.Game
{
    /**
     * Data wrapper for the Asteroid.
     **/
    public class AsteroidModel : MonoBehaviour, IPoolable<AsteroidModel>
    {
        #region Events
        public event Action<AsteroidModel> OnRemove;
        #endregion

        #region Properties
        public AsteroidFamily family;
        public AsteroidColour colour;
        #endregion

        #region Methods
        public void Detach()
        {
            OnRemove?.Invoke(this);
        }
        #endregion
    }
}