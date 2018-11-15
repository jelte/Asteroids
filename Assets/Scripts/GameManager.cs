using Asteroids.Game;
using Asteroids.Game.Data;
using Asteroids.UI;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class GameManager : MonoBehaviour
    {
        #region Static Properties
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance != null)
                {
                    return;
                }
                instance = value;
                // Persist object across scenes
                DontDestroyOnLoad(value.gameObject);
            }
        }
        #endregion
        
        #region References
        [Tooltip("Soundclip to play when player is defeated.")]
        public AudioClip defeatSound;
        [Tooltip("Soundclip to play when player is victorious.")]
        public AudioClip victorySound;
        
        // Scenes
        private GameScene mainMenuScene = new GameScene("Scenes/MainMenu");
        private GameScene gameScene = new GameScene("Scenes/Game");
        private GameScene settingsScene = new GameScene("Scenes/Settings");
        #endregion

        #region Methods
        /**
         * Load the game scene and start the game when done loading.
         **/
        public void LoadGame(ShipModel model)
        {            
            // Load the game scene
            AsyncOperation async = gameScene.Load();
            // When the game scene is loaded start the actual game
            async.completed += delegate(AsyncOperation operation) {
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                levelManager.OnGameCompleted += delegate ()
                {
                    EndGame(victorySound);
                };
                levelManager.OnGameFailure += delegate ()
                {
                    EndGame(defeatSound);
                };
                levelManager.StartGame(model);
            };
        }

        /**
         * Toggle settings interface.
         **/
        private void ToggleSettings()
        {
            // Toggle Settings scene
            AsyncOperation async = settingsScene.Toggle();

            // When the game scene is loaded start the actual game
            async.completed += delegate(AsyncOperation operation)
            {
                // Pause or Unpause the game
                Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
            };
        }

        private void EndGame(AudioClip sound)
        {
            GameAsyncOperation operation = AudioManager.Play(sound);
            operation.completed += delegate ()
            {
                InputManager.Instance.OnAnyKey += Restart;
            };
        }

        /**
         * return to main menu
         **/
        private void Restart()
        {
            // reset the ended flag.
            InputManager.Instance.OnAnyKey -= Restart;
            // Load the main menu
            mainMenuScene.Load();
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            // ensure there is only 1 game manager.
            Instance = this;
            if (Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            GetComponent<InputManager>().OnMenu += ToggleSettings;
        }        
        #endregion
    }
}
