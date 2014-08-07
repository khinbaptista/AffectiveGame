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
        # region Attributes

        protected GameMain game;

        protected Screens.Level.LevelScreen levelScreen;

        protected List<Animation> animations;

        protected Texture2D spriteSheet;

        protected Rectangle _position;

        protected bool _grounded;

        protected Vector2 movement;

        protected Vector2 inertia;

        protected float jumpSpeed = 0;

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

        # endregion



        # region Properties

        public bool grounded
        {
            get { return _grounded; }
        }

        public Vector2 position
        {
            get { return new Vector2(_position.Center.X, _position.Center.Y); }
        }

        # endregion



        # region Methods

        public Actor(GameMain game, Screens.Level.LevelScreen levelScreen)
        {
            this.levelScreen = levelScreen;

            this.game = game;
            frameInterval = TimeSpan.FromMilliseconds(150);
            frameTimer = TimeSpan.Zero;
        }

        public virtual void LoadContent(ContentManager content) { }

        public virtual void Update(GameTime gameTime)
        {
            //frameTimer = frameTimer.Add(TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds));
            frameTimer += gameTime.ElapsedGameTime;

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

        # endregion



        # region Auxiliar Methods

        /// <summary>
        /// Cancel inertia in the direction specified
        /// </summary>
        /// <param name="axis">Unit vector in the axis to be canceled (TIP: use Vector2.UnitX / Vector2.UnitY)
        /// Use Vector2.Zero to cancel movement in both directions</param>
        protected void Collide(Vector2 axis)
        {
            if (axis == Vector2.UnitX)
                inertia.X = 0;
            else if (axis == Vector2.UnitY)
                inertia.Y = 0;
            else if (axis == Vector2.Zero)
                inertia = Vector2.Zero;
        }

        # endregion
    }
}
