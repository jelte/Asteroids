using Asteroids.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.UI
{
    public class LivesDisplay : MonoBehaviour
    {
        #region References
        public RawImage image;
        #endregion

        #region Unity Methods
        void Start()
        {
            LevelManager gameManager = FindObjectOfType<LevelManager>();
            // Render all lives.
            // Note: lives + 1, as the first ship has already rendered at this point.
            for (int i = gameManager.lives + 1; i > 0; i--)
            {
                Instantiate(image, transform);
            }
            // When the ship is destroyed remove one of the lives.
            gameManager.OnShipDestroyed += delegate ()
            {
                Destroy(transform.GetChild(transform.childCount - 1).gameObject);
            };
        }
        #endregion
    }
}