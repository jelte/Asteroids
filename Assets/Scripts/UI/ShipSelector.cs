using System;
using UnityEngine;

namespace Asteroids.UI
{
    public class ShipSelector : MonoBehaviour
    {
        #region Events
        public event Action OnSelect;
        public event Action OnEnter;
        public event Action OnExit;
        #endregion

        #region Methods
        public void Highlight()
        {
            transform.localScale = Vector3.one * 1.5f;
        }

        public void StopHighlight()
        {
            transform.localScale = Vector3.one;
        }
        #endregion

        #region Unity Methods
        void OnMouseEnter()
        {
            if (Time.timeScale != 1f) return;

            OnEnter?.Invoke();
        }

        void OnMouseExit()
        {
            OnExit?.Invoke();
        }

        void OnMouseDown()
        {
            OnSelect?.Invoke();
        }
        #endregion
    }
}