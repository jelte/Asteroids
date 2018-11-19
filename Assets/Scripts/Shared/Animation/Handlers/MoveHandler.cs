using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.CommandBus;
using System.Collections;
using UnityEngine;

namespace Asteroids.Shared.Animation.Handlers
{
    public class MoveHandler : IHandler<Move>
    {
        public IEnumerator Handle(Move command)
        {
            float timer = 0;
            Vector3 startPosition = command.subject.localPosition;
            Vector3 targetPosition = startPosition + command.movement;
            while (timer < command.duration)
            {
                timer += Time.deltaTime;
                command.subject.localPosition = Vector3.Lerp(startPosition, targetPosition, timer / command.duration);
                yield return new WaitForEndOfFrame();
            }

            command.Done();
        }
    }
}
