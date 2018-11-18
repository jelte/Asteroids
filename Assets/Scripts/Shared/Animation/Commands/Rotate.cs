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
    }
}
