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
        #region Attributes

        

        #endregion

        #region Methods

        public PauseScreen(GameMain game, GameScreen father, Viewport viewport, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {
            transitionOnTime = TimeSpan.FromSeconds(0.5);
            transitionOffTime = TimeSpan.FromSeconds(0.1);

            input = new InputHandler();

            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            input.Update(gameTime);

            if (input.getStatus().Count > 0)
                FinishPauseScreen();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Finishes this pause screen managing the transition
        /// </summary>
        protected void FinishPauseScreen()
        {
            if (father != null)
                father.Unhide();

            this.ExitScreen();
        }

        #endregion
    }
}
