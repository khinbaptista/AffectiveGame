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
        private const int positionWidth = 114; // 570 / 5
        private const int positionHeight = 138; // 690 / 5
        
        private bool isFacingLeft;

        private Vector2 movement;
        private Vector2 inertia;
        private const int movementSpeed = 10;
        private float jumpSpeed;
        private const float maxJumpSpeed = 30;
        private const int collisionThreshold = movementSpeed + 2;
        private const int maxSpeed = 20;

        # endregion

        # region Methods

        public Character(Screens.Level.LevelScreen levelScreen)
            : base (levelScreen)
        {
            LoadContent(levelScreen.GetContentRef());

            action = Action.Idle;
            position = new Rectangle(0, 0, positionWidth, positionHeight);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            spriteSheet = content.Load<Texture2D>("EdonSpriteSheet");

            inertia = Vector2.Zero;

            animations = new List<Animation>();

            animations.Add(new Animation(true)); // idle
            animations.Add(new Animation(true)); // walk
            animations.Add(new Animation(true)); // jump

            animations[(int)Action.Idle].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half of image
            animations[(int)Action.Walk].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half
            animations[(int)Action.Jump].InsertFrame(new Rectangle(570, 0, 570, 690)); // 2nd half

            animations[(int)Action.Idle].InsertFrameCollider(new Rectangle(19, 43, 74, 56)); // 94, 216, 370, 280 / 5
            animations[(int)Action.Walk].InsertFrameCollider(new Rectangle(19, 43, 74, 56));
            animations[(int)Action.Jump].InsertFrameCollider(new Rectangle(36, 13, 50, 122)); // 751 / 5 - 570 / 5, 65, 249, 609 / 5
        }

        public override void HandleInput(InputHandler input)
        {
            base.HandleInput(input);

            movement = inertia;

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
                movement += new Vector2(0, maxJumpSpeed);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateFrame)
                animations[(int)action].UpdateFrame();

            // treat end of animation and input-independant status machine

            //inertia = Vector2.Zero;

            movement += levelScreen.GetGravity(); // apply gravity

            if (movement.Length() > maxSpeed)
            {
                movement.Normalize();
                movement = maxSpeed * movement;
            }
            
            position = new Rectangle(position.X + (int)movement.X, position.Y + (int)movement.Y, position.Width, position.Height);

            CheckCollisions();

            inertia = movement;
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
            animations[(int)action].Start();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            spriteBatch.Begin();
            spriteBatch.Draw(spriteSheet, position, animations[(int)action].GetFrame(), Color.White, 0, Vector2.Zero, isFacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            spriteBatch.End();
        }

        # endregion

        /// <summary>
        /// Checks for collision between the current frame and the rectangles of the level this character is in
        /// </summary>
        private void CheckCollisions()
        {
            Rectangle characterCollider = animations[(int)action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(position.X + characterCollider.X, position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            foreach (Rectangle rect in levelScreen.GetColliders())
                if (characterColliderPositioned.Intersects(rect))   // correct the position in case it collided
                {
                    // test every possible collision
                    if (characterColliderPositioned.Bottom > rect.Top && characterColliderPositioned.Top < rect.Top)
                        //&& characterColliderPositioned.Left < rect.Right && characterColliderPositioned.Right > rect.Left)
                    {
                        this.position = new Rectangle(position.X, position.Y - (characterColliderPositioned.Bottom - rect.Top), position.Width, position.Height);
                        Collide(Vector2.UnitY);

                        if (action == Action.Jump) action = Action.Idle;
                    }
                    else if (characterColliderPositioned.Top < rect.Bottom && characterColliderPositioned.Bottom > rect.Bottom)
                    //&& characterColliderPositioned.Left < rect.Right && characterColliderPositioned.Right > rect.Left)
                    {
                        this.position = new Rectangle(position.X, position.Y + (rect.Bottom - characterColliderPositioned.Top), position.Width, position.Height);
                        Collide(Vector2.UnitY);
                    }
                    else if (characterColliderPositioned.Right > rect.Left && characterColliderPositioned.Left < rect.Left)
                        //&&
                    {
                        this.position = new Rectangle(position.X - (characterColliderPositioned.Right - rect.Left), position.Y, position.Width, position.Height);
                        Collide(Vector2.UnitX);
                    }
                    else if (characterColliderPositioned.Left < rect.Right && characterColliderPositioned.Right > rect.Right)
                    {
                        this.position = new Rectangle(position.X + (rect.Right - characterColliderPositioned.Left), position.Y, position.Width, position.Height);
                        Collide(Vector2.UnitX);
                    }

                    characterColliderPositioned = new Rectangle(position.X + characterCollider.X, position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);
                }
        }

        /// <summary>
        /// Cancel inertia in the direction specified
        /// </summary>
        /// <param name="axis">Unit vector in the axis to be canceled</param>
        private void Collide(Vector2 axis)
        {
            if (axis == Vector2.UnitX)
                inertia.X = 0;
            else if (axis == Vector2.UnitY)
                inertia.Y = 0;
            else
                inertia = Vector2.Zero;
        }

        private bool Intersect(Rectangle rect)
        {
            Rectangle characterCollider = animations[(int)action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(position.X + characterCollider.X, position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            return characterColliderPositioned.Intersects(rect);

            //return animations[(int)action].GetCollider().Intersects(rect);
        }
    }
}
