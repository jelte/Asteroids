using System;
using Asteroids.Shared.CommandBus;
using UnityEngine;

namespace Asteroids.Shared.Animation.Commands
{
    public class Rotate : AbstractCommand
    {
        public Transform subject;
        public Vector3 axis;
        public float angle;
        public float duration = 1f;


        public Rotate(Transform subject, Vector3 axis, float angle) : this(subject, axis, angle, 1f)
        {

        }

        public Rotate(Transform subject, Vector3 axis, float angle, float duration)
        {
            this.subject = subject;
            this.axis = axis;
            this.angle = angle;
            this.duration = duration;
        }
    }
}
