using Asteroids.Shared.CommandBus;
using UnityEngine;

namespace Asteroids.Game.Animation.Commands
{
    public class TransitionIn : AbstractCommand
    {
        public Ship subject;

        public TransitionIn(Ship subject)
        {
            this.subject = subject;
        }
    }
}
