using Asteroids.Game.Animation.Commands;
using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.CommandBus;
using System.Collections;
using UnityEngine;

namespace Asteroids.Game.Animation.Handlers
{
    public class TransitionOutHandler : IHandler<TransitionOut>
    {
        public IEnumerator Handle(TransitionOut command)
        {
            Ship ship = command.subject;
            ship.Disable();
            Vector3 target = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1f));
            Delegation startEngine = new Delegation(delegate ()
            {
                foreach (Engine engine in ship.Engines) engine.StartEngine();
            }, 0.75f);
            startEngine.completed += delegate ()
            {
                Rotate rotation = new Rotate(ship.transform, Vector3.up, -ship.transform.localRotation.eulerAngles.y);
                rotation.completed += delegate ()
                {
                    target = (target - ship.transform.position);
                    target.Scale(Vector3.forward);
                    Move moveToEdge = new Move(ship.transform, target);
                    moveToEdge.completed += delegate ()
                    {
                        foreach (Engine engine in ship.Engines) engine.StopEngine();
                        ship.Enable();
                        command.Done();
                    };
                    Bus.Execute(moveToEdge);
                };
                Bus.Execute(rotation);
            };
            Bus.Execute(startEngine);

            yield return new WaitForEndOfFrame();
        }
    }
}
