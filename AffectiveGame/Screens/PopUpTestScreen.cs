using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AffectiveGame.Screens
{
    class PopUpTestScreen : GameScreen
    {
        private int positionX;
        private int positionY;
        //Texture2D background;

        public PopUpTestScreen(GameMain game, GameScreen father, Viewport viewport, ScreenState state = ScreenState.Active)
            : base(game, father, viewport, state)
        {
            transitionOnTime = TimeSpan.Zero;
            transitionOffTime = TimeSpan.Zero;

            isPopup = true;
        }
    }
}
