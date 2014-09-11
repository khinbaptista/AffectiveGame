using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Actors
{
    class Tree
    {
        protected List<Vector2> position;
        protected Texture2D spriteSheet;
        protected Screens.Level.LevelScreen levelScreen;

        public Tree(GameMain game, Screens.Level.LevelScreen levelScreen, List<Screens.Level.Collider> blocks)
        {
            spriteSheet = game.Content.Load<Texture2D>("normalTree");
            position = new List<Vector2>();
            this.levelScreen = levelScreen;

            foreach (Screens.Level.Collider collider in blocks)
            {
                if (collider.hasTree)
                {
                    position.Add(new Vector2(collider.GetBox().X, collider.GetBox().Y));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, levelScreen.camera.transform);
            foreach (Vector2 pos in position)
            {
                spriteBatch.Draw(spriteSheet, new Rectangle((int)(pos.X - spriteSheet.Width/2 + 15), (int)(pos.Y - spriteSheet.Height + 20), (int)spriteSheet.Width, (int)spriteSheet.Height), Color.White);
            }
            spriteBatch.End();
        }


    }
}
