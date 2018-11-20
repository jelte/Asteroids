using System;
using UnityEngine;

namespace Asteroids.Game
{
    public struct ShipPart
    {
        private Transform transform;
        private Vector3 position;
        private Quaternion rotation;
        private Rigidbody rigidbody;

        public ShipPart(Ship ship, Transform transform)
        {
            this.transform = transform;
            position = transform.localPosition;
            rotation = transform.localRotation;
            rigidbody = transform.GetComponent<Rigidbody>();
        }

        public void Restore(ShipModel model)
        {
            // re-attach to parent
            transform.parent = model.transform;
            // reset position
            transform.localPosition = position;
            // reset rotation
            transform.localRotation = rotation;

            // disable forces
            rigidbody.isKinematic = true;
            // reset velocity
            rigidbody.velocity = Vector3.zero;
        }

        public void Detach()
        {
            // Enable forces
            rigidbody.isKinematic = false;
            // Send the part flying off in a random direction
            rigidbody.AddForce(UnityEngine.Random.insideUnitSphere * 200f, ForceMode.Acceleration);
        }
    }
}