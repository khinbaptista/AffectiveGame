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
            Fall,
            Howl,
            Dig,
            Drink
        }

        // debug
        bool debug = true;
        SpriteFont font;

        public Action action { get; private set; }
        private Texture2D spriteSheet;

        Rectangle position;
        private const int positionWidth = 114; // 570 / 5
        private const int positionHeight = 138; // 690 / 5
        
        private bool isFacingLeft;
        public bool grounded { get; private set; }
        private Screens.Level.Collider lastSafeCollider;

        private Vector2 movement;
        private Vector2 inertia;
        private const int movementSpeed = 10;
        private const int howlBonusSpeed = 5;

        private float jumpSpeed = 0;
        private const float jumpSpeedStep = 15;
        private const float maxJumpSpeed = 180;
        private const float jumpHowlBoost = 50;

        private const int maxSpeed = 15;

        # endregion

        # region Methods

        public Character(Screens.Level.LevelScreen levelScreen)
            : base (levelScreen)
        {
            LoadContent(levelScreen.GetContentRef());

            action = Action.Fall;
            position = new Rectangle(100, 200, positionWidth, positionHeight);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            spriteSheet = content.Load<Texture2D>("EdonSpriteSheet");

            if (debug)
                font = content.Load<SpriteFont>("tempFont");

            inertia = Vector2.Zero;

            animations = new List<Animation>();

            animations.Add(new Animation(true)); // idle
            animations.Add(new Animation(true)); // walk
            animations.Add(new Animation(true)); // jump
            animations.Add(new Animation(true)); // fall
            animations.Add(new Animation(false)); // howl
            animations.Add(new Animation(false)); // dig
            //animations.Add(new Animation(false)); // drink

            animations[(int)Action.Idle].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half of image
            animations[(int)Action.Walk].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half
            animations[(int)Action.Jump].InsertFrame(new Rectangle(570, 0, 570, 690)); // 2nd half
            animations[(int)Action.Fall].InsertFrame(new Rectangle(570, 0, 570, 690)); // 2nd half
            animations[(int)Action.Howl].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half
            animations[(int)Action.Dig].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half

            //animations[(int)Action.Idle].InsertFrameCollider(new Rectangle(19, 43, 74, 56)); // 94, 216, 370, 280 / 5
            //animations[(int)Action.Walk].InsertFrameCollider(new Rectangle(19, 43, 74, 56));
            animations[(int)Action.Idle].InsertFrameCollider(new Rectangle(17, 79, 86, 54)); // 85, 394, 432, 271 / 5
            animations[(int)Action.Walk].InsertFrameCollider(new Rectangle(17, 79, 86, 54));

            animations[(int)Action.Jump].InsertFrameCollider(new Rectangle(36, 13, 50, 122)); // 751 / 5 - 570 / 5, 65, 249, 609 / 5
            animations[(int)Action.Fall].InsertFrameCollider(new Rectangle(36, 13, 50, 122));
            //animations[(int)Action.Howl].InsertFrameCollider(new Rectangle(19, 43, 74, 56));
            //animations[(int)Action.Dig].InsertFrameCollider(new Rectangle(19, 43, 74, 56));
            animations[(int)Action.Howl].InsertFrameCollider(new Rectangle(17, 79, 86, 54));
            animations[(int)Action.Dig].InsertFrameCollider(new Rectangle(17, 79, 86, 54));
        }

        public override void HandleInput(InputHandler input)
        {
            base.HandleInput(input);

            movement = inertia;

            AnimationControl(input);
        }

        /// <summary>
        /// Controls everything about the animation and the physics associated
        /// </summary>
        /// <param name="input"></param>
        private void AnimationControl(InputHandler input)
        {
            bool dont_move = false;

            switch (action)
            {
                case Action.Howl:
                    {
                        dont_move = true;
                        if (animations[(int)action].isFinished)
                        {
                            if (levelScreen.fullMoon || debug)
                                StartHowlBonus();
                            ChangeAction(Action.Idle);
                        }
                    } break;
                case Action.Dig:
                    {
                        dont_move = true;
                        if (animations[(int)action].isFinished)
                        {
                            lastSafeCollider.Dig();
                            ChangeAction(Action.Idle);
                        }
                    } break;
                case Action.Drink:
                    {
                        dont_move = true;
                        if (animations[(int)action].isFinished)
                        {
                            // drink
                            ChangeAction(Action.Idle);
                        }
                    } break;
                default:
                    break;
            }

            if (dont_move)
                return;

            bool moved = Move(input);

            switch (action)
            {
                case Action.Idle:
                    {
                        if (moved)
                            ChangeAction(Action.Walk);
                        if (input.WasPressed(Input.A) && grounded && CanJump())
                            ChangeAction(Action.Jump);
                        else if (input.WasPressed(Input.Y))
                            ChangeAction(Action.Dig);
                        else if (input.WasPressed(Input.B))
                            ChangeAction(Action.Howl);
                        else if (!moved)
                            movement.X = movement.X * lastSafeCollider.friction;
                    } break;

                case Action.Walk:
                    {
                        if (!moved)
                            ChangeAction(Action.Idle);
                        if (input.WasPressed(Input.A) && grounded && CanJump())
                            ChangeAction(Action.Jump);
                    } break;

                case Action.Jump:
                    {
                        if (input.Contains(Input.A))
                        {
                            jumpSpeed += jumpSpeedStep;

                            if (jumpSpeed >= maxJumpSpeed + (howlBonus ? jumpHowlBoost : 0))
                            {
                                jumpSpeed = 0;
                                ChangeAction(Action.Fall);
                            }
                        }
                        else if (input.WasReleased(Input.A))
                        {
                            jumpSpeed = 0;
                            ChangeAction(Action.Fall);
                        }

                        movement += new Vector2(0, -jumpSpeed);
                    } break;

                case Action.Fall:
                    if (grounded)
                        ChangeAction(Action.Idle);
                    break;

                default:
                    break;
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateFrame)
                animations[(int)action].UpdateFrame();

            float bonusSpeed = howlBonus ? howlBonusSpeed : 0;

            movement += levelScreen.GetGravity(); // apply gravity

            movement.X = MathHelper.Clamp(movement.X, -maxSpeed - bonusSpeed, maxSpeed + bonusSpeed);
            movement.Y = MathHelper.Clamp(movement.Y, -maxSpeed, maxSpeed);

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
                    movement += new Vector2(-movementSpeed, 0);
                else
                    isFacingLeft = true;
            }
            else if (input.Contains(Input.Right))
            {
                hasMoved = true;

                if (!isFacingLeft)
                    movement += new Vector2(movementSpeed, 0);
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

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, levelScreen.camera.transform);
            spriteBatch.Draw(spriteSheet, position, animations[(int)action].GetFrame(), Color.White, 0, Vector2.Zero, isFacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            spriteBatch.End();

            if (debug)
            {
                spriteBatch.Begin();

                spriteBatch.DrawString(font, "Action: " + action.ToString(), new Vector2(50, 50), Color.White);
                spriteBatch.DrawString(font, "Grounded: " + grounded.ToString(), new Vector2(50, 70), Color.White);
                spriteBatch.DrawString(font, "Howl bonus: " + howlBonus.ToString(), new Vector2(50, 110), Color.White);
                spriteBatch.DrawString(font, "Camera zoom: " + levelScreen.camera.zoom, new Vector2(50, 130), Color.White);

                if (lastSafeCollider != null)
                    spriteBatch.DrawString(font, "Friction: " + lastSafeCollider.friction, new Vector2(50, 90), Color.White);

                spriteBatch.End();
            }
        }

        # endregion

        /// <summary>
        /// Checks for collision between the current frame and the rectangles of the level this character is in
        /// </summary>
        private void CheckCollisions()
        {
            Rectangle characterCollider = animations[(int)action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(position.X + characterCollider.X, position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            grounded = false;

            foreach (Screens.Level.Collider col in levelScreen.GetColliders())
            {
                Rectangle rect = col.GetBox();

                if (col.isActive && characterColliderPositioned.Intersects(rect))   // correct the position in case it collided
                {
                    if (col.isHarmful)
                    {
                        // check damage and everything
                        //return;
                    }

                    // test every possible collision
                    if (characterColliderPositioned.Bottom > rect.Top && characterColliderPositioned.Top < rect.Top//)
                    && characterColliderPositioned.Left < rect.Right && characterColliderPositioned.Right > rect.Left)
                    {
                        // The flickering when jumping is due to the imperfections in the spritesheet, which means it will (hopefully) work just fine when we change the assets
                        this.position = new Rectangle(position.X, position.Y - (characterColliderPositioned.Bottom - rect.Top), position.Width, position.Height);
                        Collide(Vector2.UnitY);
                        lastSafeCollider = col;
                        grounded = true;
                    }
                    else if (characterColliderPositioned.Top < rect.Bottom && characterColliderPositioned.Bottom > rect.Bottom)
                    //&& characterColliderPositioned.Left < rect.Right && characterColliderPositioned.Right > rect.Left)
                    {
                        this.position = new Rectangle(position.X, position.Y + (rect.Bottom - characterColliderPositioned.Top), position.Width, position.Height);
                        Collide(Vector2.UnitY);
                        ChangeAction(Action.Fall);
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
        }

        /// <summary>
        /// Checks whether or not there is room above Edon for him to perform a jump
        /// </summary>
        private bool CanJump()
        {
            bool canJump = true;

            Rectangle characterCollider = animations[(int)action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(position.X + characterCollider.X,
                position.Y + characterCollider.Y - characterCollider.Height, characterCollider.Width, characterCollider.Height);

            foreach (Screens.Level.Collider collider in levelScreen.GetColliders())
                if (collider.GetBox().Intersects(characterColliderPositioned))
                    canJump = false;

            return canJump;
        }

        /// <summary>
        /// Cancel inertia in the direction specified
        /// </summary>
        /// <param name="axis">Unit vector in the axis to be canceled (TIP: use Vector2.UnitX / Vector2.UnitY)</param>
        private void Collide(Vector2 axis)
        {
            if (axis == Vector2.UnitX)
                inertia.X = 0;
            else if (axis == Vector2.UnitY)
                inertia.Y = 0;
            else
                inertia = Vector2.Zero;
        }

        public Vector2 GetPosition() { return new Vector2(position.Center.X, position.Center.Y); }
    }
}
