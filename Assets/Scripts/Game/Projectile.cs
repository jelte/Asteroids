using System.Collections;
using UnityEngine;

namespace Asteroids.Game
{
    public class Projectile : MonoBehaviour
    {
        #region Properties
        [Tooltip("Base projectile speed.")]
        public float projectileSpeed = 400f;
        [Tooltip("Duration the projectile exists.")]
        public float ttl = 10f;
        #endregion

        #region Methods
        /**
         * Add force to the projectile.
         **/
        public void Launch(float shipSpeed)
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * (projectileSpeed + shipSpeed), ForceMode.Impulse);
        }

        /**
         * Clean up the projectile after the ttl has past.
         **/
        IEnumerator CleanUp()
        {
            yield return new WaitForSeconds(ttl);
            DestroyImmediate(gameObject);
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            // Schedule the cleanUp
            StartCoroutine(CleanUp());
        }

        void OnCollisionEnter(Collision collision)
        {
            // destroy the projectile when colliding with another object
            Destroy(gameObject);
        }
        #endregion

    }
}