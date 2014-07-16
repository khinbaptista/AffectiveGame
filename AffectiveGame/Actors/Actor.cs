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

        public Actor(Screens.Level.LevelScreen levelScreen)
        {
            this.levelScreen = levelScreen;

            frameInterval = TimeSpan.FromMilliseconds(150);
            frameTimer = TimeSpan.Zero;
        }

        public virtual void LoadContent(ContentManager content) { }

        public virtual void Update(GameTime gameTime)
        {
            frameTimer = frameTimer.Add(TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds));

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
    }
}
