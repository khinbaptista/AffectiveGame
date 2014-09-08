using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Actors
{
    class TreeOnFire : Actor
    {
        public TreeOnFire(GameMain game, Screens.Level.LevelScreen levelScreen)
            : base(game, levelScreen)
        {
            spriteSheet = game.Content.Load<Texture2D>("treeOnFire");
            animations = new List<Animation>();

            Animation anim = new Animation();

            this.frameInterval = TimeSpan.FromMilliseconds(300);

            int frameWidth = spriteSheet.Width / 3;
            int scale = 1;

            _position = new Rectangle(0, 0, frameWidth * scale, spriteSheet.Height * scale);

            anim.InsertFrame(new Rectangle(0, 0, frameWidth, spriteSheet.Height));
            anim.InsertFrame(new Rectangle(frameWidth, 0, frameWidth, spriteSheet.Height));
            anim.InsertFrame(new Rectangle(frameWidth * 2, 0, frameWidth, spriteSheet.Height));

            animations.Add(anim);
            animations[0].Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateFrame)
                animations[0].UpdateFrame();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, levelScreen.camera.transform);
            spriteBatch.Draw(spriteSheet, _position, animations[0].GetFrame(), Color.White);
            spriteBatch.End();
        }

    }
}
