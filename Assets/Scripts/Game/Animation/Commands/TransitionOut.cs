using Asteroids.Shared.CommandBus;
using UnityEngine;

namespace Asteroids.Game.Animation.Commands
{
    public class TransitionOut : AbstractCommand
    {
        public Ship subject;

        public TransitionOut(Ship subject)
        {
            this.subject = subject;
        }
    }
}
