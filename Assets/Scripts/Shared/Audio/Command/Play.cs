using Asteroids.Shared.CommandBus;
using UnityEngine;

namespace Asteroids.Shared.Audio.Command
{
    public class Play : AbstractCommand
    {
        public AudioClip clip;
        public SoundType type;

        public Play(AudioClip clip) : this(clip, SoundType.SoundFX)
        {
        }

        public Play(AudioClip clip, SoundType type)
        {
            this.clip = clip;
            this.type = type;
        }
    }
}
