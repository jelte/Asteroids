using Asteroids;
using Asteroids.Shared.Audio;
using Asteroids.Shared.Audio.Command;
using Asteroids.Shared.CommandBus;
using System.Collections;
using UnityEngine;

namespace Asteroids.Shared.Audio.Handler
{
    public class TransitoinInHandler : IHandler<Play>
    {
        public IEnumerator Handle(Play command)
        {
            AudioSource source = AudioManager.GetSource();
            source.volume = command.type == SoundType.SoundFX ? AudioManager.SoundVolume : AudioManager.MusicVolume;
            source.clip = command.clip;
            source.Play();

            if (command.type == SoundType.Music)
            {
                source.playOnAwake = true;
                command.Done();
                yield break;
            }
            source.playOnAwake = false;

            yield return new WaitForSeconds(command.clip.length);

            AudioManager.Recycle(source);

            command.Done();
        }
    }
}
