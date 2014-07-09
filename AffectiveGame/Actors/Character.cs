using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace AffectiveGame.Actors
{
    class Character : Actor//, Interfaces.ISoundSource
    {
        # region Attributes

        public enum Action
        {
            Idle = 0,
            Walk,
            Jump,
            Howl,
            Dig,
            Drink
        }

        public Action action { get; private set; }
        List<Animation> animations;

        Rectangle position;
        int frameIndex;
        bool isFacingLeft;

        # endregion

        # region Methods



        # endregion

        /*
        # region Interface implementations

        public void Play(int soundIndex)
        {

        }

        # endregion
        */
    }
}
