using System;

namespace Asteroids.Shared.CommandBus
{
    public interface ICommand
    {
        int Callbacks { set; }

        event Action completed;

        void Done();
    }
    
}
