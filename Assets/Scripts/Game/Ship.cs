using Asteroids.Shared.Inputs;
using Asteroids.Shared.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ship : MonoBehaviour
    {
        #region Events
        public event Action OnCollision;
        public event Action OnFire;
        public event Action OnAccelerationStart;
        public event Action OnAccelerationStop;

        public event Action<float> OnTurn;
        #endregion

        #region Properties
        [Range(1f, 360f), Tooltip("Turn speed of the ship.")]
        public float turnSpeed = 30f;
        [Range(0.1f, 500f), Tooltip("Acceleration speed of the ship.")]
        public float accelerationSpeed = 10f;

        // is the ship accelerating
        private bool isAccelerating = false;
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

        private new MeshCollider collider;
        
        public ShipModel Model
        {
            set
            {
                model = Instantiate(value, transform);
                model.Bind(this);

                collider = GetComponentInChildren<MeshCollider>();
                Engines = GetComponentsInChildren<Engine>();
            }
        }

        public IEnumerable<Engine> Engines { get; internal set; }
        #endregion

        #region Methods
        public void Accelerate(float acceleration)
        {
            // there is no reverse in Asteroids
            if (acceleration <= 0)
            {
                if (isAccelerating)
                {
                    isAccelerating = false;
                    OnAccelerationStop?.Invoke();
                }
                return;
            }

            if (rigidbody.velocity.normalized != transform.forward)
            {
                acceleration *= 2;
            }
            // Add force to the ship in the forward direction.
            rigidbody.AddForce(transform.forward * acceleration * accelerationSpeed, ForceMode.Force);

            if (!isAccelerating)
            {
                isAccelerating = true;
                OnAccelerationStart?.Invoke();
            }
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

        public void Enable()
        {
            collider.enabled = true;

            // Make the ship vulnerable after 1 second.
            StartCoroutine(MakeVulnerable(1f));

            InputManager.Instance.OnHorizontalAxis += Turn;
            InputManager.Instance.OnVerticalAxis += Accelerate;
            InputManager.Instance.OnFire += Fire;
        }

        public void Disable()
        {
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
            }

            vulnerable = false;

            InputManager.Instance.OnHorizontalAxis -= Turn;
            InputManager.Instance.OnVerticalAxis -= Accelerate;
            InputManager.Instance.OnFire -= Fire;
        }

        #endregion

        #region Unity Methods
        void Awake()
        {         
            // Make sure there is a rigidbody attached to the ship and that it is configured properly.
            rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.useGravity = false;
            if (rigidbody.constraints != (RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation))
            {
                Debug.LogWarning("Check pitch and roll restraints on ship");
            }

            projectilePool = ObjectPoolFactory.Get(projectilePrefab);
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
            Disable();
        }
        #endregion
    }
}
