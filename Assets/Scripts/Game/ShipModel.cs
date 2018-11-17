using Asteroids.Shared;
using UnityEngine;

namespace Asteroids.Game
{
    public class ShipModel : MonoBehaviour
    {
        #region Properties
        public float projectileOffset = 2.5f;
        [Range(0f, 1f)]
        public float rollSpeed = .25f;
        [Range(0f, 90f)]
        public float rollDistance = 30f;
        #endregion

        #region References
        public Explosion explosionEffect;
        public AudioClip explosionSound;
        public AudioClip projectileSound;

        // Explosion pool
        private IObjectPool<Explosion> explosionPool;
        #endregion

        #region Methods
        public void Bind(Ship ship)
        {
            ship.OnFire += Fire;
            ship.OnCollision += Explode;
            ship.OnTurn += Turn;

            foreach (Engine engine in GetComponentsInChildren<Engine>())
            {
                ship.OnAccelerationStart += engine.StartEngine;
                ship.OnAccelerationStop += engine.StopEngine;
            }
        }

        void Explode()
        {
            // Break the model;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform part = transform.GetChild(i);
                // Assign to space instead of ship
                part.parent = transform.parent.parent;
                Rigidbody rigidbody = part.gameObject.GetComponent<Rigidbody>();
                // Enable forces
                rigidbody.isKinematic = false;
                // Send the part flying off in a random direction
                rigidbody.AddForce(Random.insideUnitSphere * 200f, ForceMode.Acceleration);
                // Remove the part after 2 seconds
                Destroy(part.gameObject, 2f);
            }
            // Create the explosion effect
            explosionPool.Get(transform.position);
            // Play the explosion sound
            AudioManager.Play(explosionSound);
            // Destroy the ship model
            Destroy(gameObject);
        }

        void Fire()
        {
            // Play the projectile sound
            AudioManager.Play(projectileSound);
        }

        void Turn(float delta)
        {
            // determine the angle
            float direction = delta == 0 ? 0 : delta < 0 ? 1 : -1;

            // Determine the final desired angle.
            Quaternion targetAngle = Quaternion.Euler(Vector3.forward * direction * rollDistance);

            // pivot toward the targt angle.
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetAngle, rollSpeed);
        }
        #endregion

        #region Unity Methods
        void Awake()
        {
            explosionPool = ObjectPoolFactory.Get(explosionEffect);
        }
        #endregion
    }
}