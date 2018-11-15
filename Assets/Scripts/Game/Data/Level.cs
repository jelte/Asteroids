using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Game.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Level")]
    public class Level : ScriptableObject
    {
        public List<Wave> waves;

        public int AsteroidCount
        {
            get
            {
                int asteroidCount = 0;
                waves.ForEach(delegate (Wave wave) { asteroidCount += wave.quantity; });
                return asteroidCount * 7;
            }
        }
    }
}