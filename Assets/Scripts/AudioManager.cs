using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class AudioManager : MonoBehaviour
    {
        #region Static Properties
        private static AudioManager instance;

        public static AudioManager Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance != null) return;
                // Set the value
                instance = value;
                // Persist object across scenes
                DontDestroyOnLoad(value.gameObject);
            }
        }
        #endregion

        #region Static Properties
        public static float MusicVolume
        {
            get { return Instance.overall * Instance.music; }
        }

        public static float SoundVolume
        {
            get { return Instance.overall * Instance.sound; }
        }
        #endregion

        #region Static Methods
        /**
         * Play an audio clip
         **/
        public static GameAsyncOperation Play(AudioClip audioClip)
        {
            GameAsyncOperation operation = new GameAsyncOperation();

            Instance.StartCoroutine(Play(operation, audioClip));

            return operation;
        }

        private static IEnumerator Play(GameAsyncOperation operation, AudioClip audioClip)
        {
            // play the clip
            AudioSource.PlayClipAtPoint(audioClip, Vector3.zero, SoundVolume);

            // wait for it to finish
            yield return new WaitForSeconds(audioClip.length);

            // flag the operation as completed
            operation.Done();
        }
        #endregion

        #region Properties
        private float overall = 1f;
        private float music = 0.0225f;
        private float sound = 1f;
        
        public float Music
        {
            get { return music; }
            set
            {
                music = value;
                PlayerPrefs.SetFloat("Music", music);
                musicPlayer.volume = MusicVolume;
            }
        }
        public float Sound
        {
            get { return sound; }
            set
            {
                sound = value;
                PlayerPrefs.SetFloat("Sound", sound);
            }
        }
        public float Overall
        {
            get { return overall; }
            set
            {
                overall = value;
                PlayerPrefs.SetFloat("Overall", overall);
                musicPlayer.volume = MusicVolume;
            }
        }
        #endregion

        #region References
        private AudioSource musicPlayer;
        #endregion

        #region Unity Methods
        void Start()
        {
            // ensure there is only 1 audio manager.
            Instance = this;
            if (Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            // Load volumes from settings.
            overall = PlayerPrefs.GetFloat("Audio", overall);
            music = PlayerPrefs.GetFloat("Music", music);
            sound = PlayerPrefs.GetFloat("Sound", sound);

            // adjust the music volume
            musicPlayer = GetComponent<AudioSource>();
            musicPlayer.volume = MusicVolume;
            // Start playing
            musicPlayer.Play();
        }
        #endregion
    }
}