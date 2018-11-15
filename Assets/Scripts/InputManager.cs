using System;
using UnityEngine;

namespace Asteroids
{
    public class InputManager : MonoBehaviour
    {
        #region Static Properties
        private static InputManager instance;

        public static InputManager Instance
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

        #region Events
        public event Action OnMenu;
        public event Action OnSubmit;
        public event Action OnAnyKey;
        public event Action OnFire;

        public event Action<float> OnHorizontalAxis;
        public event Action<float> OnVerticalAxis;
        #endregion

        #region Unity Methods
        void Start()
        {
            // ensure there is only 1 input manager.
            Instance = this;
            if (Instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        void Update()
        {
            // Handle keyboard input
            if (Input.GetButtonDown("Fire1"))
            {
                OnFire?.Invoke();
            }
            if (Input.GetButtonDown("Menu"))
            {
                OnMenu?.Invoke();
            }
            if (Input.GetButtonDown("Submit"))
            {
                OnSubmit?.Invoke();
            }
            if (Input.anyKeyDown)
            {
                OnAnyKey?.Invoke();
            }
            OnHorizontalAxis?.Invoke(Input.GetAxis("Horizontal") * Time.deltaTime);
            OnVerticalAxis?.Invoke(Input.GetAxis("Vertical") * Time.deltaTime);
        }
        #endregion
    }
}
