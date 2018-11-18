using Asteroids.Game;
using System;
using UnityEngine;

namespace Asteroids.UI
{
    public class ShipSelector : MonoBehaviour
    {
        #region Events
        public event Action<int> OnSelect;
        public event Action<int> OnEnter;
        #endregion

        #region Properties
        public int index;
        #endregion

        #region Unity Methods
        void OnMouseOver()
        {
            OnEnter?.Invoke(index);
        }

        void OnMouseDown()
        {
            OnSelect?.Invoke(index);
        }
        #endregion
    }
}