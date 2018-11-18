using Asteroids.Game;
using Asteroids.Shared;
using Asteroids.Shared.Audio.Command;
using Asteroids.Shared.CommandBus;
using UnityEngine;

namespace Asteroids
{
    public class GameManager : MonoBehaviour
    {
        #region Static Properties
        public static GameManager Instance { get; private set; }
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

            Play command = new Play(sound);
            command.completed += delegate ()
            {
                InputManager.Instance.OnAnyKey += Restart;
            };
            Bus.Execute(command);
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
        void Awake()
        {
            // ensure there is only 1 game manager.
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            // assign the instance
            Instance = this;

            // Persist object across scenes
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Application.targetFrameRate = 600;

            InputManager.Instance.OnMenu += ToggleSettings;
        }        
        #endregion
    }
}
