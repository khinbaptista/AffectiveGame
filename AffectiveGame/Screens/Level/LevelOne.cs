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
        protected Texture2D ground;
        protected Actors.Fire flame;

        public LevelOne(GameMain game, GameScreen father, float gravitySpeed = 300, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, gravitySpeed, state)
        {
            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            ground = content.Load<Texture2D>("stoneGround");
            flame = new Actors.Fire(game, this);
            flame.position = new Vector2(300, 600);

            //environmentColliders = new List<Collider>();
            //fearArea = new List<Rectangle>();
            LevelFromFile(Environment.CurrentDirectory + @"\gapsize2.txt");
            startPosition = new Vector2(startZone.X, startZone.Y);
            Edon = new Actors.Character(game, this, startPosition);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            flame.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            flame.Draw(spriteBatch, gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, camera.transform);

            for (int i = 0; i < environmentColliders.Count; i++)
            {
                if (environmentColliders[i].isActive)
                    spriteBatch.Draw(ground, environmentColliders[i].GetBox(), Color.White);
                //spriteBatch.Draw(blank, environmentColliders[i].GetBox(), new Color(0.6f, 0.5f, 0.2f, 0.0f));
            }

            // debug
            foreach (Rectangle fearZone in fearArea)
                spriteBatch.Draw(blank, fearZone, new Color(0.3f, 0, 0, 0.5f));

            foreach (Rectangle moonTrigger in moonTriggers)
                spriteBatch.Draw(blank, moonTrigger, new Color(1, 1, 0.5f, 0.5f));

            spriteBatch.Draw(blank, startZone, new Color(0, 0.2f, 0.4f, 0.5f));
            spriteBatch.Draw(blank, endZone, new Color(0.4f, 0.1f, 0, 0.5f));
            
            spriteBatch.End();
        }

    }
}
