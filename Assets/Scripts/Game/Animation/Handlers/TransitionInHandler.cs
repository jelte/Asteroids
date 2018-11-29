using Asteroids.Game.Animation.Commands;
using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.CommandBus;
using System.Collections;
using UnityEngine;

namespace Asteroids.Game.Animation.Handlers
{
    public class TransitionInHandler : IHandler<TransitionIn>
    {
        public IEnumerator Handle(TransitionIn command)
        {
            Ship ship = command.subject;
            ship.Disable();
            
            ship.transform.localPosition = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.05f));
            Delegation startEngines = new Delegation(delegate ()
            {
                foreach (Engine engine in ship.Engines) engine.StartEngine();
            }, 0.75f);
            startEngines.completed += delegate ()
            {
                Move moveToCenter = new Move(ship.transform, -ship.transform.localPosition);
                moveToCenter.completed += delegate ()
                {
                    foreach (Engine engine in ship.Engines) engine.StopEngine();
                    ship.Enable();
                    command.Done();
                };
                Bus.Execute(moveToCenter);
            };
            Bus.Execute(startEngines);

            yield return new WaitForEndOfFrame();
        }
    }
}
