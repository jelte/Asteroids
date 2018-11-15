using System;

namespace Asteroids.Game.Data
{
    [Serializable]
    public struct Wave
    {
        public AsteroidFamily family;
        public AsteroidColour colour;
        public float speed;
        public int quantity;
        public float delay;
    }
}