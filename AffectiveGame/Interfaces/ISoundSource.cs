using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace AffectiveGame.Interfaces
{
    interface ISoundSource
    {
        List<SoundEffect> soundEffects;

        void Play(int effectIndex);
    }
}
