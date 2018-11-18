using System;
using UnityEngine;

namespace Asteroids.Shared.Inputs
{
    /**
     * Add mobile support.
     * 
     * Todo: add properties to settings.
     **/
    public class MobileInputManager : MonoBehaviour, IInputManager
    {
        #region Events
        public event Action<float> OnHorizontalAxis;
        public event Action<float> OnVerticalAxis;

        public event Action OnFire;
        public event Action OnAnyKey;
        public event Action OnSubmit;
        public event Action OnMenu;
        #endregion

        #region Properties
        private float turnDeadzone = 30f;
        private float turnSensitivity = 100f;
        private float accelerationDeadzone = 30f;
        private float accelerationSensitivity = 60f;

        private Vector2 movement;
        #endregion

        #region Methods
        void HandleTouch(Touch touch)
        {
            if (Time.timeScale == 0f) return;

            if (touch.position.x <= Screen.width / 2)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        movement = Vector2.zero;
                        break;
                    case TouchPhase.Moved:
                        movement += touch.deltaPosition;
                        break;
                    case TouchPhase.Ended:
                        movement = Vector2.zero;
                        break;
                }

                // Take deadzone into account
                float deltaX = Mathf.Abs(movement.x) >= turnDeadzone ? movement.x : 0f;
                OnHorizontalAxis?.Invoke(Mathf.Clamp(deltaX / turnSensitivity, -1f, 1f) * Time.deltaTime);

                // Take deadzone into account
                float deltaY = Mathf.Abs(movement.y) >= accelerationDeadzone ? movement.y : 0f;
                OnVerticalAxis?.Invoke(Mathf.Clamp(deltaY / accelerationSensitivity, -1f, 1f) * Time.deltaTime);
            }
            else if (touch.phase == TouchPhase.Began)
            {
                OnFire?.Invoke();
            }

        }
        #endregion

        #region Unity Methods
        private void Update()
        {
            if (Input.touchCount > 0)
            {
                HandleTouch(Input.GetTouch(0));

                if (Input.touchCount > 1)
                {
                    HandleTouch(Input.GetTouch(1));
                }

                OnAnyKey?.Invoke();
            }

            // call menu on back.
            if (Input.GetKeyUp("escape"))
            {
                OnMenu?.Invoke();
            }
        }
        #endregion
    }
}
