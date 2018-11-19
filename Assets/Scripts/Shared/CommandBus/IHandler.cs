using System.Collections;

namespace Asteroids.Shared.CommandBus
{
    public interface IHandler { }

    public interface IHandler<T> : IHandler where T : ICommand
    {
        IEnumerator Handle(T command);
    }
}
