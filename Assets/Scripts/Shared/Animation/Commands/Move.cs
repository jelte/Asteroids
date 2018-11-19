using System;
using Asteroids.Shared.CommandBus;
using UnityEngine;

namespace Asteroids.Shared.Animation.Commands
{
    public class Move : AbstractCommand
    {
        public Transform subject;
        public Vector3 movement;
        public float duration = 1f;

        public Move(Transform subject, Vector3 movement) : this(subject, movement, 1f)
        {

        }

        public Move(Transform subject, Vector3 movement, float duration)
        {
            this.subject = subject;
            this.movement = movement;
            this.duration = duration;
        }
    }
}
