using Asteroids.Shared.CommandBus;
using System;

namespace Asteroids.Shared.Animation.Commands
{
    public class Delegation : AbstractCommand
    {
        public Action action;

        public float duration;

        public Delegation(Action action) : this(action, 0f)
        {

        }

        public Delegation(Action action, float duration)
        {
            this.action = action;
            this.duration = duration;
        }
    }
}
