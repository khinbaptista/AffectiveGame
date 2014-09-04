using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Screens
{
    /// <summary>
    /// Transition screen between two other screens
    /// </summary>
    class TransitionScreen : GameScreen
    {
        private GameScreen next;
        private TimeSpan duration;
        private TimeSpan counter;

        /// <summary>
        /// Creates a new instance of transition screen
        /// </summary>
        /// <param name="nextScreen">The screen to be created after this transition ends</param>
        /// <param name="duration">Duration of this transition in miliseconds</param>
        public TransitionScreen(GameMain game, GameScreen nextScreen, int duration = 500)
            : base (game, null, ScreenState.TransitionOn)
        {
            transitionOnTime = TimeSpan.FromMilliseconds(500);
            transitionOffTime = TimeSpan.FromMilliseconds(500);
            next = nextScreen;

            this.duration = TimeSpan.FromMilliseconds(duration);
            counter = TimeSpan.Zero;

            LoadContent(game.Content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (screenState == ScreenState.Active)
            {
                counter += gameTime.ElapsedGameTime;

                if (counter > duration)
                {
                    this.ExitScreen();
                    game.AddScreen(next);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(blank, new Rectangle(0, 0, game.viewport.Width, game.viewport.Height), Color.Black);
            spriteBatch.End();
        }
    }
}
