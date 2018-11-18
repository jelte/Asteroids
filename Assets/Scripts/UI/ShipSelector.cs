using Asteroids.Game;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Asteroids.UI
{
    public class ShipSelector : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        #region Events
        public event Action<int> OnSelect;
        public event Action<int> OnEnter;
        public event Action<float> OnSwipe;
        #endregion

        #region Properties
        private int index;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            OnSelect?.Invoke(index);
        }

        void Start()
        {
            index = transform.GetSiblingIndex();
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnSwipe?.Invoke(-eventData.scrollDelta.x);
        }
        #endregion
    }
}