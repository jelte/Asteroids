using Asteroids.Shared.CommandBus;
using System;

namespace Asteroids.Shared.Animation.Commands
{
    public class Delegation : AbstractCommand
    {
        public Action action;

        public float duration;
    }
}
