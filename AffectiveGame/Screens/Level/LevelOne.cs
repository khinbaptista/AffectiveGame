using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Screens.Level
{
    class LevelOne : LevelScreen
    {
        public LevelOne(GameMain game, GameScreen father, Viewport viewport, float gravitySpeed = 1, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, gravitySpeed, state)
        {
            LoadContent(game.Content);

            Edon = new Actors.Character(this);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            environmentColliders = new List<Rectangle>();
            environmentColliders.Add(new Rectangle(-50, viewport.Height - 80, viewport.Width + 100, 80)); // ground
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(blank, environmentColliders[0], new Color(0.6f, 0.5f, 0.2f, 0.0f));
            spriteBatch.End();
        }


    }
}
