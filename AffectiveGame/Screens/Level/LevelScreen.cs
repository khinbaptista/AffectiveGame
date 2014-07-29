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
        protected readonly Vector2 gravitySpeed;
        protected Actors.Character Edon;
        protected List<Collider> environmentColliders;
        
        public bool fullMoon { get; protected set; }
        
        protected Texture2D background;

        public LevelScreen(GameMain game, GameScreen father, Viewport viewport, float gravitySpeed, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {
            this.gravitySpeed = new Vector2(0, gravitySpeed);

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

            Edon.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            Edon.HandleInput(input);

            if (input.getStatus().Contains(Input.LeftBumper))
            {
                game.AddScreen(new MainMenuScreen(game, null, viewport));
                this.ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            
            if (screenState == ScreenState.Hidden)
                return;
            
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            spriteBatch.End();

            Edon.Draw(spriteBatch, gameTime);
        }

        public ContentManager GetContentRef() { return game.Content; }

        public List<Collider> GetColliders() { return environmentColliders; }

        public Vector2 GetGravity() { return gravitySpeed; }
    }
}
