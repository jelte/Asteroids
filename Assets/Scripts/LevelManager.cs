using Asteroids.Game;
using Asteroids.Game.Data;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class LevelManager : MonoBehaviour
    {
        #region Events
        public Action OnLevelComplete;
        public Action OnShipDestroyed;
        public Action OnGameCompleted;
        public Action OnGameFailure;
        #endregion

        #region Properties

        [Tooltip("Number of lives the player starts with")]
        public int lives = 3;
        [Tooltip("How long does it take to respawn")]
        public float respawnTime = 2f;
        // Current Level
        private int level = 0;
        // Number of active asteroids
        private int asteroids = 0;
        #endregion

        #region References
        // The Ship prefab
        [Tooltip("Ship Prefab")]
        public Ship shipPrefab;

        // The selected ship model
        private ShipModel shipModel;

        // All available levels
        private Level[] levels;

        private AsteroidManager asteroidManager;
        #endregion

        #region Methods
        /**
         * Start the game.
         **/
        public void StartGame(ShipModel shipModel)
        {
            // Load all levels (if too many change to load on demand).
            levels = Resources.LoadAll<Level>("Levels");

            // Set the ship model
            this.shipModel = shipModel;

            // Setup the asteroid manager
            asteroidManager = FindObjectOfType<AsteroidManager>();
            asteroidManager.OnAsteroidDestroy += delegate () {
                // All astroid destroyed ? => level completed.
                if (--asteroids == 0)
                {
                    OnLevelComplete?.Invoke();
                }
            };

            // Listen for the level to be completed to load the next level.
            OnLevelComplete += LoadLevel;

            // Create a copy of the player model for the player lives indicator.
            Instantiate(shipModel, GameObject.FindGameObjectWithTag("Lives").transform);

            // Load the first level
            LoadLevel();
            // Spawn the ship
            SpawnShip();
        }

        /**
         * Load the next level
         **/
        private void LoadLevel()
        {
            // game is completed when there are no more levels
            if (level >= levels.Length)
            {
                OnGameCompleted?.Invoke();
                return;
            }
            // Load the level
            asteroidManager.Load(levels[level]);
            // keep track of how many asteroids are/will be in the level
            asteroids = levels[level].AsteroidCount;

            // make sure the next level is loaded next.
            level++;
        }

        /**
         * Spawn a ship
         **/
        private void SpawnShip()
        {
            Ship ship = Instantiate(shipPrefab, asteroidManager.transform);
            ship.Model = shipModel;

            ship.OnCollision += delegate () { OnShipDestroyed?.Invoke(); };
            if (--lives == 0)
            {
                // No more lives, Game ends in failure
                ship.OnCollision += delegate () { OnGameFailure?.Invoke(); };
            }
            else
            {
                // Respawn after 2 seconds
                ship.OnCollision += delegate () { StartCoroutine(Respawn(2f)); };
            }
        }

        /**
         * Respawn a ship
         **/
        private IEnumerator Respawn(float delay)
        {
            // Wait for delay to pass
            yield return new WaitForSeconds(delay);

            // Spawn a new ship
            SpawnShip();
        }
        #endregion
    }
}
