using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.CommandBus;
using System;
using System.Collections;
using UnityEngine;

namespace Asteroids.Shared.Animation.Handlers
{
    class RotateHandler : IHandler<Rotate>
    {
        public IEnumerator Handle(Rotate command)
        {
            float timer = 0;
            Quaternion startRotation = command.subject.localRotation;
            Quaternion targetRotation = Quaternion.Euler(command.subject.localEulerAngles + Quaternion.AngleAxis(command.angle, command.axis).eulerAngles);
            while (timer < command.duration)
            {
                timer += Time.deltaTime;
                command.subject.localRotation = Quaternion.Slerp(startRotation, targetRotation, timer / command.duration);
                yield return new WaitForEndOfFrame();
            }

            command.Done();
        }
    }
}
