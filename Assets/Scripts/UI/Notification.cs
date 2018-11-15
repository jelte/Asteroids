using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.UI
{
    public class Notification : MonoBehaviour
    {
        [Tooltip("Message shown when the player runs out of lives.")]
        public string gameOverMessage = "GAME OVER";
        [Tooltip("Message shown when the player finishes all levels.")]
        public string gameCompletedMessage = "SECTOR CLEARED !";

        #region Methods
        private void Show(string message)
        {
            Text text = GetComponent<Text>();
            // set the message.
            text.text = message;

            // show all texts
            foreach (Text component in GetComponentsInChildren<Text>())
            {
                component.enabled = true;
            }
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            LevelManager gameManager = FindObjectOfType<LevelManager>();
            gameManager.OnGameFailure += delegate () {
                Show(gameOverMessage);
            };
            gameManager.OnGameCompleted += delegate () {
                Show(gameCompletedMessage);
            };
        }
        #endregion
    }
}
