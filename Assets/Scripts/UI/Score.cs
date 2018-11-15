using Asteroids.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.UI
{
    public class Score : MonoBehaviour
    {
        #region Properties
        // track the score.
        private int score = 0;
        #endregion

        #region References
        private Text textField;
        #endregion

        #region Method
        void IncreaseScore() {
            // increase score by 10.
            score += 10;

            // set display.
            textField.text = score.ToString();
        }
        #endregion

        #region Unity Methods
        void Start()
        {
            textField = GetComponent<Text>();

            FindObjectOfType<AsteroidManager>().OnAsteroidDestroy += IncreaseScore;
        }
        #endregion
    }
}