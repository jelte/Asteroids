using System;
using UnityEngine;

namespace Asteroids.Shared.CommandBus
{
    public class AbstractCommand : ICommand
    {
        #region Events
        public event Action completed;
        #endregion

        #region Properties
        public int Callbacks
        {
            get;  set;
        }
        #endregion

        #region Methods
        public void Done()
        {
            if (--Callbacks == 0)
            {
                completed?.Invoke();
            }
        }
        #endregion
    }
}