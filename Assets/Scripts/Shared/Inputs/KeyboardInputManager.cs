using System;
using UnityEngine;

namespace Asteroids.Shared.Inputs
{
    public class KeyboardInputManager : MonoBehaviour, IInputManager
    {
        #region Events
        public event Action<float> OnHorizontalAxis;
        public event Action<float> OnVerticalAxis;
        public event Action OnFire;
        public event Action OnSubmit;
        public event Action OnMenu;
        public event Action OnAnyKey;
        #endregion

        #region Unity Methods
        void Update()
        {
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
