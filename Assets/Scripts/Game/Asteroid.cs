using Asteroids.Shared;
using UnityEngine;

namespace Asteroids.Game
{
    public class Asteroid : MonoBehaviour, IPoolable<Asteroid>
    {
        #region Events
        public event System.Action<Asteroid> OnRemove;
        #endregion

        #region Properties        
        public float Velocity { get { return rigidbody.velocity.magnitude; } }
        #endregion

        #region References
        private new Rigidbody rigidbody;
        [Tooltip("Sound the asteroids makes when destroyed")]
        public AudioClip explosion;
        #endregion

        #region Methods
        /**
         * Initialize the asteroid 
         **/
        public void Init(int size, Vector3 direction)
        {          
            // Rescale according to size
            Vector3 scale = transform.localScale;
            transform.localScale = scale / (4 - size);
            
            // Rotate the asteroid randomly around its axis
            rigidbody.angularVelocity = Random.insideUnitSphere * 1.5f;

            // Reset velocity
            rigidbody.velocity = Vector3.zero;

            // Add a continuous force in the direction.
            rigidbody.AddForce(direction, ForceMode.Acceleration);
        }
        #endregion

        #region Unity Methods
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        
        void OnCollisionEnter(Collision collision)
        {
            AudioManager.Play(explosion);

            // Destroy the asteroid
            OnRemove?.Invoke(this);
        }
        #endregion
    }
}
