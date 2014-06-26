using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Screens
{
    class PauseScreen : GameScreen
    {
        public PauseScreen(GameMain game, GameScreen father, Viewport viewport, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {

        }
    }
}
