using UnityEngine;

namespace Asteroids.Game
{
    public class Asteroid : MonoBehaviour
    {
        #region Events
        public event System.Action OnDestroy;
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
        public void Init(int size, Transform parent, Vector3 direction)
        {
            transform.parent = parent;
            // Rescale according to size
            Vector3 scale = transform.localScale;
            transform.localScale = scale / (4 - size);

            rigidbody = GetComponent<Rigidbody>();
            // Rotate the asteroid randomly around its axis
            rigidbody.angularVelocity = Random.insideUnitSphere * 1.5f;

            // Add a continuous force in the direction.
            rigidbody.AddForce(direction, ForceMode.Acceleration);
        }
        #endregion

        #region Unity Methods
        void OnCollisionEnter(Collision collision)
        {
            OnDestroy?.Invoke();

            AudioManager.Play(explosion);

            // Destroy the asteroid
            Destroy(gameObject);
        }
        #endregion
    }
}
