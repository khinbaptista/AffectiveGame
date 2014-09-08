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
        protected Texture2D grassGround;
        protected Texture2D stoneGround;
        protected Texture2D blankSprite;
        protected Actors.Fire flame;

        public LevelOne(GameMain game, GameScreen father, float gravitySpeed = 300, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, gravitySpeed, state)
        {
            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            grassGround = content.Load<Texture2D>("stoneGround");
            stoneGround = content.Load<Texture2D>("stone");
            blankSprite = content.Load<Texture2D>("blank");
            flame = new Actors.Fire(game, this);
            flame.position = new Vector2(300, 700);

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

            Texture2D usedSprite = grassGround;
            for (int i = 0; i < environmentColliders.Count; i++)
            {
                switch (environmentColliders[i].sprite)
                {
                    case 0:
                        usedSprite = blankSprite;
                        break;
                    case 1:
                        usedSprite = grassGround;
                        break;
                    case 2:
                        usedSprite = stoneGround;
                        break;
                }
                if (environmentColliders[i].isActive)
                    spriteBatch.Draw(usedSprite, environmentColliders[i].GetBox(), Color.White);
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
