using Asteroids.Shared.Animation.Commands;
using Asteroids.Shared.CommandBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Shared.Animation.Handlers
{
    public class DelegationHandler : IHandler<Delegation>
    {
        public IEnumerator Handle(Delegation command)
        {
            command.action.Invoke();
            yield return new WaitForSeconds(command.duration);
            
            command.Done();
        }
    }
}