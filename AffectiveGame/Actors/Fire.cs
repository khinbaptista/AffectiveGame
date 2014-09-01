using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Actors
{
    class Fire : Actor
    {
        public Fire(GameMain game, Screens.Level.LevelScreen levelScreen)
            : base(game, levelScreen)
        {
            spriteSheet = game.Content.Load<Texture2D>("flame");
            animations = new List<Animation>();

            Animation anim = new Animation();

            int frameWidth = spriteSheet.Width / 4;
            anim.InsertFrame(new Rectangle(0, 0, frameWidth, spriteSheet.Height));
            anim.InsertFrame(new Rectangle(frameWidth, 0, frameWidth, spriteSheet.Height));
            anim.InsertFrame(new Rectangle(frameWidth * 2, 0, frameWidth, spriteSheet.Height));
            anim.InsertFrame(new Rectangle(frameWidth * 3, 0, frameWidth, spriteSheet.Height));

            animations.Add(anim);
            animations[0].Start();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            if (updateFrame)
            {
                animations[0].UpdateFrame();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, levelScreen.camera.transform);
            spriteBatch.Draw(spriteSheet, _position, animations[0].GetFrame(), Color.White);
            spriteBatch.End();
        }

    }
}
