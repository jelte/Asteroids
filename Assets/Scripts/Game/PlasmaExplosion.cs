using System;
using Asteroids.Shared.Pooling;
using UnityEngine;

namespace Asteroids.Game
{
    public class PlasmaExplosion : MonoBehaviour, IPoolable<PlasmaExplosion>
    {
        public event Action<PlasmaExplosion> OnRemove;
    }
}