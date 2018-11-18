using System;
using UnityEngine;

namespace Asteroids
{
    public class InputManager : MonoBehaviour
    {
        #region Static Properties
        public static InputManager Instance { get; private set; }
        #endregion

        #region Events
        public event Action OnMenu;
        public event Action OnSubmit;
        public event Action OnAnyKey;
        public event Action OnFire;

        public event Action<float> OnHorizontalAxis;
        public event Action<float> OnVerticalAxis;
        #endregion

        #region Properties
        private float turnDeadzone = 30f;
        private float turnSensitivity = 60f;
        private float accelerationDeadzone = 30f;
        private float accelerationSensitivity = 60f;
        private Vector2 start;
        private Vector2 movement;
        #endregion

        #region Unity Methods
        void Awake()
        {
            // ensure there is only 1 input manager.
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

        void Update()
        {
#if UNITY_ANDROID
            if (Input.touchCount > 0) {
                Touch touch0 = Input.GetTouch(0);
                switch (touch0.phase)
                {
                    case TouchPhase.Began:
                        movement = Vector2.zero;
                        break;
                    case TouchPhase.Moved:
                        movement += touch0.deltaPosition;
                        break;
                    case TouchPhase.Ended:
                        movement = Vector2.zero;
                        break;
                }
                
                float deltaX = Mathf.Abs(movement.x) >= turnDeadzone ? movement.x : 0f;
                OnHorizontalAxis?.Invoke(Mathf.Clamp(deltaX / turnSensitivity, -1f, 1f) * Time.deltaTime);

                float deltaY = Mathf.Abs(movement.y) >= accelerationDeadzone ? movement.y : 0f;
                OnVerticalAxis?.Invoke(Mathf.Clamp(movement.y / accelerationSensitivity, -1f, 1f) * Time.deltaTime);

                if (Input.touchCount > 1)
                {
                    Touch touch1 = Input.GetTouch(1);
                    if (touch1.phase == TouchPhase.Began)
                    {
                        OnFire?.Invoke();
                    }
                }
                
                OnAnyKey?.Invoke();
            }
#else
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
#endif
        }
        #endregion
    }
}
