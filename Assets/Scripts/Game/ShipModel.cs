using Asteroids.Shared.Audio.Command;
using Asteroids.Shared.CommandBus;
using Asteroids.Shared.Pooling;
using System;
using System.Collections.Generic;
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

        private List<Engine> engines;
        // note: replace tuple with dedicated Part struct.
        private List<ShipPart> parts;

        // Explosion pool
        private IObjectPool<Explosion> explosionPool;
        #endregion

        #region Methods
        public void Bind(Ship ship)
        {
            ship.OnFire += Fire;
            ship.OnCollision += Explode;
            ship.OnTurn += Turn;

            foreach (Engine engine in engines)
            {
                ship.OnAccelerationStart += engine.StartEngine;
                ship.OnAccelerationStop += engine.StopEngine;
            }
        }

        void Explode()
        {

            foreach (ShipPart part in parts) {
                part.Detach();
            }
            // Create the explosion effect
            explosionPool.Get(transform.position);
            // Play the explosion sound
            Bus.Execute(new Play(explosionSound));
            // Destroy the ship model
            Destroy(gameObject);
        }

        void Fire()
        {
            // Play the projectile sound
            Bus.Execute(new Play(projectileSound));
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

        void Reconstruct()
        {
            parts.ForEach(delegate (ShipPart part)
            {
                part.Restore(this);
            });
        }
        #endregion

        #region Unity Methods
        void Awake()
        {
            explosionPool = ObjectPoolFactory.Get(explosionEffect);
            
            parts = new List<ShipPart>();
            engines = new List<Engine>();
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                Transform child = transform.GetChild(i);

                Engine engine = child.GetComponent<Engine>();
                if (engine != null)
                {
                    engines.Add(engine);
                }

                // Store reference to ship part, its position and rotation.
                parts.Add(new ShipPart(child));
            }
        }
        #endregion
    }
}