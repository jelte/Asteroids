using Asteroids.Shared;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids.Game
{
    public class Explosion : MonoBehaviour, IPoolable<Explosion>
    {
        #region events
        public event Action<Explosion> OnRemove;
        #endregion

        #region Properties
        [Tooltip("How long does the explosion last?")]
        public float explosionTime = 4f;
        #endregion

        #region Methods
        IEnumerator CleanUp()
        {
            yield return new WaitForSeconds(explosionTime);

            OnRemove?.Invoke(this);
        }
        #endregion

        #region Unity Methods
        void OnEnable()
        {
            StartCoroutine(CleanUp());
        }
        #endregion
    }
}