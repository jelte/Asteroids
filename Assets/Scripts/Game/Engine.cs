using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Game
{
    public class Engine : MonoBehaviour
    {
        #region References
        private new ParticleSystem particleSystem;
        #endregion

        #region Methods
        public void StartEngine()
        {
            particleSystem.Play();
        }

        public void StopEngine()
        {
            particleSystem.Stop();
        }
        #endregion

        #region Unity Methods
        void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }
        #endregion

    }
}