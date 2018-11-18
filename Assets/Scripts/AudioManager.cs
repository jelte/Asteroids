using Asteroids.Shared;
using Asteroids.Shared.Audio.Command;
using Asteroids.Shared.CommandBus;
using Asteroids.Shared.Pooling;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class AudioManager : MonoBehaviour
    {
        #region Static Properties
        public static AudioManager Instance { get; private set; }
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
        public static AudioSource GetSource()
        {
            return Instance.sources.Get();
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
        private ComponentPool<AudioSource> sources;
        #endregion

        #region Unity Methods
        void Awake()
        {
            // ensure there is only 1 audio manager.
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            // Set the value
            Instance = this;
            // Persist object across scenes
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            sources = new ComponentPool<AudioSource>(gameObject);

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