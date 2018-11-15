using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.UI
{
    public class AudioSettings : MonoBehaviour
    {
        #region Unity Methods
        void Start()
        {
            AudioManager audioManager = FindObjectOfType<AudioManager>();

            // Retrieve all sliders
            Slider[] sliders = GetComponentsInChildren<Slider>();

            // initialize value and onChange listeners for each slider.
            foreach(Slider slider in sliders)
            {
                switch (slider.transform.parent.gameObject.name)
                {
                    case "General":
                        slider.value = audioManager.Overall;
                        slider.onValueChanged.AddListener(delegate (float value)
                        {
                            audioManager.Overall = value;
                        });
                        break;
                    case "Music":
                        slider.value = audioManager.Music;
                        slider.onValueChanged.AddListener(delegate (float value)
                        {
                            audioManager.Music = value;
                        });
                        break;
                    case "SoundFX":
                        slider.value = audioManager.Sound;
                        slider.onValueChanged.AddListener(delegate (float value)
                        {
                            audioManager.Sound = value;
                        });
                        break;
                }
            }
        }
        #endregion
    }
}