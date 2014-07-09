using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Screens.Level
{
    abstract class LevelScreen : GameScreen
    {
        protected readonly float gravitySpeed;
        protected readonly Actors.Character Edon;

        public LevelScreen(GameMain game, GameScreen father, Viewport viewport, float gravitySpeed = 5, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {
            this.gravitySpeed = gravitySpeed;
            Edon = new Actors.Character();

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

            Edon.HandleInput(input);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

        }
    }
}
