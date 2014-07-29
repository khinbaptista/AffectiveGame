using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Actors
{
    abstract class Actor
    {
        protected Screens.Level.LevelScreen levelScreen;
        protected List<Animation> animations;

        /// <summary>
        /// Time it takes to move to the next frame in the animation
        /// </summary>
        protected readonly TimeSpan frameInterval;

        /// <summary>
        /// Controls the time 
        /// </summary>
        protected TimeSpan frameTimer;

        /// <summary>
        /// Indicated whether or not the frame should be updated
        /// </summary>
        protected bool updateFrame;

        /// <summary>
        /// Bonus you get when you howl at the moon
        /// </summary>
        protected bool howlBonus;

        /// <summary>
        /// Duration of the bonus you get for howling at the moon
        /// </summary>
        protected readonly TimeSpan howlBonusDuration;

        /// <summary>
        /// Timer to control the howl bonus
        /// </summary>
        protected TimeSpan howlBonusTimer;

        protected bool howlBonusEnded;

        public Actor(Screens.Level.LevelScreen levelScreen)
        {
            this.levelScreen = levelScreen;

            frameInterval = TimeSpan.FromMilliseconds(150);
            frameTimer = TimeSpan.Zero;
            howlBonusDuration = TimeSpan.FromMilliseconds(5000);
        }

        public virtual void LoadContent(ContentManager content) { }

        public virtual void Update(GameTime gameTime)
        {
            frameTimer = frameTimer.Add(TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds));

            if (howlBonus)
            {
                howlBonusTimer = howlBonusTimer.Add(TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds));

                if (howlBonusTimer.TotalMilliseconds > howlBonusDuration.TotalMilliseconds)
                {
                    howlBonusTimer = TimeSpan.Zero;
                    howlBonus = false;
                    howlBonusEnded = true;
                }
            }

            if (frameTimer.TotalMilliseconds > frameInterval.TotalMilliseconds)
            {
                frameTimer = TimeSpan.Zero;
                updateFrame = true;
            }
            else if (updateFrame)
                updateFrame = false;
        }

        public virtual void HandleInput(InputHandler input) { }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }

        protected void StartHowlBonus()
        {
            if (howlBonus) return;

            howlBonus = true;
            howlBonusTimer = TimeSpan.Zero;
            howlBonusEnded = false;
        }
    }
}
