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
    class Character : Actor
    {
        # region Attributes

        public enum Action
        {
            Idle = 0,
            Walk,
            Jump,
            Howl,
            Dig,
            Drink
        }

        public Action action { get; private set; }
        List<Animation> animations;
        private Texture2D spriteSheet;

        Rectangle position;
        //int frameIndex; // not neccessary anymore - frame control is now inside the animation, which makes a lot more sense.
        
        private bool isFacingLeft;

        # endregion

        # region Methods

        public Character(Screens.Level.LevelScreen levelScreen)
            : base (levelScreen)
        {
            LoadContent(levelScreen.GetContentRef());
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            spriteSheet = content.Load<Texture2D>("");

            animations.Add(new Animation(true)); // idle
            animations.Add(new Animation(true)); // walk
            animations.Add(new Animation(true)); // jump

            animations[(int)Action.Idle].InsertFrame(new Rectangle(94, 216, 370, 280));
            animations[(int)Action.Walk].InsertFrame(new Rectangle(94, 216, 370, 280));
            animations[(int)Action.Jump].InsertFrame(new Rectangle(751, 65, 249, 609));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateFrame)
                animations[(int)action].UpdateFrame();

            // treat end of animation and input-independant status machine
        }

        public override void HandleInput(InputHandler input)
        {
            base.HandleInput(input);

            if (action == Action.Idle)
            {
                if (input.Contains(Input.Left))
                    if (!isFacingLeft)
                        isFacingLeft = true;
                    else
                        action = Action.Walk;
                else if (input.Contains(Input.Right))
                    if (isFacingLeft)
                        isFacingLeft = false;
                    else
                        action = Action.Walk;
                else if (input.Contains(Input.A))
                    action = Action.Jump;
            }
            else if (action == Action.Walk)
            {
                if (!Move(input))
                {
                    if (input.Contains(Input.A))
                        action = Action.Jump;
                    else
                        action = Action.Idle;
                }
            }
            else if (action == Action.Jump)
            {
                if (!Move(input))
                {
                    if (input.Contains(Input.A))
                    {
                        //********************************************
                    }
                }
            }
        }

        private bool Move(InputHandler input)
        {
            bool hasMoved = false;

            if (input.Contains(Input.Left))
            {
                hasMoved = true;

                if (isFacingLeft)
                {
                    // move left
                }
                else
                    isFacingLeft = true;
            }
            else if (input.Contains(Input.Right))
            {
                hasMoved = true;

                if (!isFacingLeft)
                {
                    // move right
                }
                else
                    isFacingLeft = false;
            }

            return hasMoved;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        # endregion
    }
}
