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
        public LevelOne(GameMain game, GameScreen father, Viewport viewport, float gravitySpeed = 400, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, gravitySpeed, state)
        {
            startPosition = new Vector2(100, 400);
            Edon = new Actors.Character(game, this, startPosition);
            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            environmentColliders = new List<Collider>();
            fearArea = new List<Rectangle>();
            LevelFromFile(Environment.CurrentDirectory + @"\levelOne.txt");

            /*
            environmentColliders.Add(new Collider(-500, 1080 - 80, 1920 + 1000, 80)); // ground
            environmentColliders.Add(new Collider(-500, 1080 - 800, 510, 800));
            environmentColliders.Add(new Collider(1920 - 10, 1080 - 800, 300, 800));
            environmentColliders.Add(new Collider(150, 850, 500, 20, false, true, false, 0.5f));*/
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

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            for (int i = 0; i < environmentColliders.Count; i++)
                if (environmentColliders[i].isActive)
                    spriteBatch.Draw(blank, environmentColliders[i].GetBox(), new Color(0.6f, 0.5f, 0.2f, 0.0f));
            
            spriteBatch.End();
        }


    }
}
