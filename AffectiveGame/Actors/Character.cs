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
        
        private bool isFacingLeft;

        private Vector2 movement;
        private const float movementSpeed = 10;
        private float jumpSpeed;
        private const float maxJumpSpeed = 30;

        # endregion

        # region Methods

        public Character(Screens.Level.LevelScreen levelScreen)
            : base (levelScreen)
        {
            LoadContent(levelScreen.GetContentRef());

            position = new Rectangle(0, 0, 50, 50);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            spriteSheet = content.Load<Texture2D>("");

            movement = Vector2.Zero;
            
            animations.Add(new Animation(true)); // idle
            animations.Add(new Animation(true)); // walk
            animations.Add(new Animation(true)); // jump
            
            animations[(int)Action.Idle].InsertFrame(new Rectangle(94, 216, 370, 280)); // yeah this won't work - those are the colliders, not the frames (they also will be scaled)
            animations[(int)Action.Walk].InsertFrame(new Rectangle(94, 216, 370, 280));
            animations[(int)Action.Jump].InsertFrame(new Rectangle(751, 65, 249, 609));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateFrame)
                animations[(int)action].UpdateFrame();

            // treat end of animation and input-independant status machine

            position.Offset((int)movement.X, (int)movement.Y);
            movement = Vector2.Zero;
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
                        ChangeAction(Action.Walk);
                else if (input.Contains(Input.Right))
                    if (isFacingLeft)
                        isFacingLeft = false;
                    else
                        ChangeAction(Action.Walk);
                else if (input.Contains(Input.A))
                    ChangeAction(Action.Jump);
            }
            else if (action == Action.Walk)
            {
                if (!Move(input))
                {
                    if (input.Contains(Input.A))
                        ChangeAction(Action.Jump);
                    else
                        ChangeAction(Action.Idle);
                }
            }
            else if (action == Action.Jump)
            {
                
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
                    movement += new Vector2(-movementSpeed, 0);
                }
                else
                    isFacingLeft = true;
            }
            else if (input.Contains(Input.Right))
            {
                hasMoved = true;

                if (!isFacingLeft)
                {
                    movement += new Vector2(movementSpeed, 0);
                }
                else
                    isFacingLeft = false;
            }

            return hasMoved;
        }

        private void ChangeAction(Action newAction)
        {
            action = newAction;
            animations[(int)action].Play();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        # endregion
    }
}
