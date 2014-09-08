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
        private GameScreen.ScreenType next;
        private TimeSpan duration;
        private TimeSpan counter;

        /// <summary>
        /// Creates a new instance of transition screen
        /// </summary>
        /// <param name="nextScreen">The screen to be created after this transition ends</param>
        /// <param name="duration">Duration of this transition in miliseconds</param>
        public TransitionScreen(GameMain game, ScreenType nextScreen, int duration = 500)
            : base (game, null, ScreenState.TransitionOn)
        {
            transitionOnTime = TimeSpan.FromMilliseconds(500);
            transitionOffTime = TimeSpan.FromMilliseconds(500);

            this.duration = TimeSpan.FromMilliseconds(duration);
            counter = TimeSpan.Zero;
            next = nextScreen;

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
                    GameScreen newScreen;
                    switch (next)
                    {
                        case ScreenType.MainMenu:
                            newScreen = new MainMenuScreen(game, null); break;
                        case ScreenType.LevelOne:
                            newScreen = new Level.LevelOne(game, null); break;
                        default:
                            newScreen = new MainMenuScreen(game, null); break;
                    }

                    this.ExitScreen();
                    game.AddScreen(newScreen);
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
