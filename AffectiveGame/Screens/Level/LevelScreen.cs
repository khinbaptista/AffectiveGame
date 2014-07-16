using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Screens.Level
{
    class LevelScreen : GameScreen
    {
        protected readonly float gravitySpeed;
        protected readonly Actors.Character Edon;
        
        protected Texture2D background;

        public LevelScreen(GameMain game, GameScreen father, Viewport viewport, float gravitySpeed = 5, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {
            this.gravitySpeed = gravitySpeed;
            Edon = new Actors.Character(this);

            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            background = content.Load<Texture2D>("Sky");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            Edon.HandleInput(input);

            if (input.getStatus().Count != 0)
            {
                game.AddScreen(new MainMenuScreen(game, null, viewport));
                this.ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();
        }

        public ContentManager GetContentRef() { return game.Content; }
    }
}
