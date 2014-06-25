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
        private SpriteFont font;
        private Rectangle backgroundRect;
        private Texture2D background;

        public PopUpTestScreen(GameMain game, GameScreen father, Viewport viewport,
            string text, string textButtonA, string textButtonB, Rectangle position)
            : base(game, father, viewport, ScreenState.Active)
        {
            transitionOnTime = TimeSpan.Zero;
            transitionOffTime = TimeSpan.Zero;

            isPopup = true;

            backgroundRect = position;

            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            font = content.Load<SpriteFont>("tempFont");
            background = content.Load<Texture2D>("blank");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(InputHandler input)
        {
            base.HandleInput(input);

            if (input.IsNewStatus(Input.B))
            {
                father.ToggleUnderneath();
                this.KillScreen();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            Color textColor = Color.Black;

            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundRect, Color.Red);
            spriteBatch.DrawString(font, "Press B to go back", new Vector2(100, 100), textColor);
            spriteBatch.End();
        }
    }
}
