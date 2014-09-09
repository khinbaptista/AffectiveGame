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
        protected Vector2 lCloudPos;
        protected Vector2 rCloudPos;
        protected TimeSpan frameTimer;
        protected readonly TimeSpan durationTimer;
        protected float moonSpeed = 15;
        protected bool moonAnimation = false;
        protected bool fullMoon = false;
        protected SoundEffect howling;
        protected bool howlPlayed = false;
        
        public Moon(GameMain game, Screens.Level.LevelScreen levelScreen)
        {
            this.levelScreen = levelScreen;
            this.game = game;

            durationTimer = TimeSpan.FromSeconds(10);

            moonPos = new Vector2(700, 100);
            lCloudPos = new Vector2(600, 80);
            rCloudPos = new Vector2(750, 120);

            _moon = new Rectangle((int)moonPos.X, (int)moonPos.Y, 200, 200);
            leftCloud = new Rectangle((int)lCloudPos.X, (int)lCloudPos.Y, 244, 156);
            rightCloud = new Rectangle((int)rCloudPos.X, (int)rCloudPos.Y, 244, 156);
        }

        public void LoadContent(ContentManager content)
        {
            howling = game.Content.Load<SoundEffect>("howling");
            moonSprite = content.Load<Texture2D>("moon");
            cloudSprite = content.Load<Texture2D>("cloud");
            frameTimer = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            float finalSpeed = (float)(moonSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (moonAnimation)
            {
                frameTimer += gameTime.ElapsedGameTime;
                if (frameTimer.TotalMilliseconds < durationTimer.TotalMilliseconds)
                {
                    lCloudPos.X -= finalSpeed;
                    rCloudPos.X += finalSpeed;
                    leftCloud.X = (int)lCloudPos.X;
                    rightCloud.X = (int)rCloudPos.X;
                }
                else if ((frameTimer.TotalMilliseconds > 2*(durationTimer.TotalMilliseconds)) && (frameTimer.TotalMilliseconds < 3*(durationTimer.TotalMilliseconds)))
                {
                    lCloudPos.X += finalSpeed;
                    rCloudPos.X -= finalSpeed;
                    leftCloud.X = (int)lCloudPos.X;
                    rightCloud.X = (int)rCloudPos.X;
                }
                else if ((frameTimer.TotalMilliseconds > 3*(durationTimer.TotalMilliseconds)))
                {
                    frameTimer = TimeSpan.Zero;
                    moonAnimation = false;
                }

                if (!((_moon.Intersects(leftCloud)) && (_moon.Intersects(rightCloud))))
                    fullMoon = true;
                else
                    fullMoon = false;
            }

            if(fullMoon && !howlPlayed)
                playHowling();

            if (!fullMoon)
                howlPlayed = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(moonSprite, _moon, Color.White);
            spriteBatch.Draw(cloudSprite, leftCloud, Color.White);
            spriteBatch.Draw(cloudSprite, rightCloud, Color.White);
            spriteBatch.End();
        }

        public void startFullMoon()
        {
            moonAnimation = true;
        }

        public bool getMoonValue()
        {
            return fullMoon;
        }

        public void playHowling()
        {
            howling.Play();
            howlPlayed = true;
        }

    }
}
