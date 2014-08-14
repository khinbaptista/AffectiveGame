using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace AffectiveGame.Actors
{
    class Moon
    {
        protected GameMain game;
        protected Screens.Level.LevelScreen levelScreen;
        protected Texture2D moonSprite;
        protected Texture2D cloudSprite;
        Rectangle _moon;
        Rectangle leftCloud;
        Rectangle rightCloud;
        protected Vector2 moonPos;
        protected int moonWidth;
        protected int moonHeight;
        

        public Moon(GameMain game, Screens.Level.LevelScreen levelScreen)
        {
            this.levelScreen = levelScreen;
            this.game = game;

            _moon = new Rectangle((int)moonPos.X, (int)moonPos.Y, moonWidth, moonHeight);
        }

        public void LoadContent(ContentManager content)
        {
            moonSprite = content.Load<Texture2D>("blank");
            cloudSprite = content.Load<Texture2D>("blank");

        }

        public void Update()
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, levelScreen.camera.transform);
            //spriteBatch.Draw();
            spriteBatch.End();
        }

    }
}
