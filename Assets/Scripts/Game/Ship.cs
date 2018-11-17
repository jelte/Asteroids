using Asteroids.Shared;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids.Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ship : MonoBehaviour
    {
        #region Events
        public event Action OnCollision;
        public event Action OnFire;
        public event Action<float> OnTurn;
        #endregion

        #region Properties
        [Range(1f, 360f), Tooltip("Turn speed of the ship.")]
        public float turnSpeed = 30f;
        [Range(0.1f, 500f), Tooltip("Acceleration speed of the ship.")]
        public float accelerationSpeed = 10f;

        // is the ship destroyable by collisions.
        private bool vulnerable = false;
        // Get the position of the ship.
        public Vector3 Position { get { return transform.position; } }
        #endregion

        #region References
        [Tooltip("Prefab for the projectile.")]
        public Projectile projectilePrefab;
        // projectile pool
        private IObjectPool<Projectile> projectilePool; 
        // model of the ship
        private ShipModel model;
        private new Rigidbody rigidbody;

        public ShipModel Model
        {
            set
            {
                model = Instantiate(value, transform);
                model.Bind(this);
            }
        }
        #endregion

        #region Methods
        public void Accelerate(float acceleration)
        {
            // there is no reverse in Asteroids
            if (acceleration <= 0) return;

            if (rigidbody.velocity.normalized != transform.forward)
            {
                acceleration *= 2;
            }
            // Add force to the ship in the forward direction.
            rigidbody.AddForce(transform.forward * acceleration * accelerationSpeed, ForceMode.Force);
        }

        public void Turn(float yaw)
        {
            // Turn the ship
            transform.Rotate(transform.up * yaw * turnSpeed, Space.Self);

            OnTurn?.Invoke(yaw);
        }

        public void Fire()
        {
            // notify event listeners
            OnFire?.Invoke();

            // Spawn the projectile
            Projectile projectile = projectilePool.Get(
                transform.localPosition + transform.forward * model.projectileOffset,
                transform.rotation,
                transform.parent
            );

            // Launch the projectile
            projectile.Launch(rigidbody.velocity.magnitude);
        }

        /**
         * Make the ship Vulnerable after the delay. 
         **/
        IEnumerator MakeVulnerable(float delay)
        {
            yield return new WaitForSeconds(delay);
            vulnerable = true;
        }
        #endregion

        #region unity
        void Start()
        {
            projectilePool = ObjectPoolFactory.Get(projectilePrefab);

            // Make sure there is a rigidbody attached to the ship and that it is configured properly.
            rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null) { 
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.useGravity = false;
            if (rigidbody.constraints != (RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation))
            {
                Debug.LogWarning("Check pitch and roll restraints on ship");
            }

            // Make the ship vulnerable after 1 second.
            StartCoroutine(MakeVulnerable(1f));

            InputManager inputManager = FindObjectOfType<InputManager>();
            inputManager.OnHorizontalAxis += Turn;
            inputManager.OnVerticalAxis += Accelerate;
            inputManager.OnFire += Fire;
        }
        
        void OnCollisionEnter(Collision collision)
        {
            // Don't do something when the ship isn't vulnerable.
            if (!vulnerable) return;

            // notify event listeners.
            OnCollision?.Invoke();
            
            // Destroy the ship.
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            // Clean Up input manager;
            InputManager.Instance.OnHorizontalAxis -= Turn;
            InputManager.Instance.OnVerticalAxis -= Accelerate;
            InputManager.Instance.OnFire -= Fire;
        }
        #endregion
    }
}
