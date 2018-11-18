using Asteroids.Shared.Pooling;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids.Game
{
    public class Projectile : MonoBehaviour, IPoolable<Projectile>
    {
        #region Events
        public event Action<Projectile> OnRemove;
        #endregion

        #region Properties
        [Tooltip("Base projectile speed.")]
        public float projectileSpeed = 400f;
        [Tooltip("Duration the projectile exists.")]
        public float ttl = 10f;
        #endregion

        #region References
        private new Rigidbody rigidbody;
        private IEnumerator cleanUp;
        #endregion

        #region Methods
        /**
         * Add force to the projectile.
         **/
        public void Launch(float shipSpeed)
        {
            // Reset movement (due to object pooling)
            rigidbody.velocity = Vector3.zero;

            rigidbody.AddForce(transform.forward * (projectileSpeed + shipSpeed), ForceMode.Impulse);
        }

        /**
         * Clean up the projectile after the ttl has past.
         **/
        IEnumerator CleanUp()
        {
            yield return new WaitForSeconds(ttl);

            OnRemove?.Invoke(this);
        }
        #endregion

        #region Unity Methods
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            // Schedule the cleanUp
            cleanUp = CleanUp();
            StartCoroutine(cleanUp);
        }

        void OnCollisionEnter(Collision collision)
        {
            // Stop the pending scheduled clean up
            StopCoroutine(cleanUp);

            // disable the projectile when colliding with another object
            OnRemove?.Invoke(this);
        }
        #endregion
    }
}