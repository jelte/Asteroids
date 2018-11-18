using System;

namespace Asteroids.Shared.Inputs
{
    public interface IInputManager
    {
        #region Events
        event Action<float> OnHorizontalAxis;
        event Action<float> OnVerticalAxis;

        event Action OnFire;
        event Action OnSubmit;
        event Action OnMenu;
        event Action OnAnyKey;
        #endregion
    }
}