using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace AffectiveGame.Actors
{
    class Character : Actor
    {
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

        # region Attributes

        // debug
        bool debug = true;
        SpriteFont font;
        bool backToStart = true;

        private Action _action;

        # region Position

        private const int positionWidth = 114; // 570 / 5
        private const int positionHeight = 138; // 690 / 5
        private bool isFacingLeft;
        private Screens.Level.Collider _lastSafeCollider;

        # endregion

        // Movement constants (must be updated to real time) [ friction fixed ]
        # region Movement constants
        
        private const float movementSpeed = 100;
        private const float jumpSpeedStep = 150;
        private const float maxJumpSpeed = 2000;
        private const int maxSpeed = 700;

        # endregion

        # region Howl bonus

        /// <summary>
        /// Bonus you get when you howl at the moon
        /// </summary>
        private bool howlBonus;

        /// <summary>
        /// Duration of the bonus you get for howling at the moon
        /// </summary>
        private readonly TimeSpan howlBonusDuration;

        /// <summary>
        /// Timer to control the howl bonus
        /// </summary>
        private TimeSpan howlBonusTimer;

        private const int howlBonusSpeed = 50;

        private const float jumpHowlBoost = 500;

        private bool howlBonusEnded;

        # endregion

        # region Fear onus

        private float _fear;
        private bool _afraid;
        private const float fearThreshold = 128;
        private const float fearMaxValue = 300;
        private const float _fearRegenerationRate = 32;
        private const int fearOnusSpeed = 100;
        private const int fearOnusJump = 300;
        private const int drinkReduceFear = 75;

        # endregion

        # endregion


        # region Properties

        public Action action
        {
            get { return _action; }
        }

        public Screens.Level.Collider lastSafeCollider
        {
            get { return _lastSafeCollider; }
            set { _lastSafeCollider = value; }
        }

        # endregion


        # region Methods

        public Character(GameMain game, Screens.Level.LevelScreen levelScreen, Vector2 position)   // 100 500
            : base (game, levelScreen)
        {
            LoadContent(game.Content);

            _action = Action.Fall;
            _position = new Rectangle((int)position.X, (int)position.Y, positionWidth, positionHeight);
            howlBonusDuration = TimeSpan.FromMilliseconds(5000);
        }

        # region Override

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
            animations.Add(new Animation(false)); // drink

            LoadAnimationsFromFile(Environment.CurrentDirectory + @"\EdonAnim.txt");
        }

        public override void HandleInput(InputHandler input)
        {
            base.HandleInput(input);

            movement = inertia;

            AnimationControl(input);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateFrame)
                animations[(int)_action].UpdateFrame();

            if (howlBonus)
            {
                howlBonusTimer = howlBonusTimer + gameTime.ElapsedGameTime;

                if (howlBonusTimer > howlBonusDuration)
                {
                    howlBonusTimer = TimeSpan.Zero;
                    howlBonus = false;
                    howlBonusEnded = true;
                }
            }

            float speedModifier = howlBonus ? howlBonusSpeed : 0;
                    speedModifier -= _afraid ? fearOnusSpeed : 0;

            
            movement += levelScreen.GetGravity();

            movement.X = MathHelper.Clamp(movement.X, -maxSpeed - speedModifier, maxSpeed + speedModifier);
            movement.Y = MathHelper.Clamp(movement.Y, -maxSpeed, maxSpeed);

            inertia = movement;
            movement *= game.deltaTime;

            _position = new Rectangle(_position.X + (int)(movement.X), _position.Y + (int)(movement.Y), _position.Width, _position.Height);

            CheckCollisionSAT();

            Rectangle characterCollider = animations[(int)_action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(_position.X + characterCollider.X, _position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            UpdateFear(characterColliderPositioned);
            UpdateMoonTrigger(characterColliderPositioned);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, levelScreen.camera.transform);
            spriteBatch.Draw(spriteSheet, _position, animations[(int)_action].GetFrame(), Color.White, 0, Vector2.Zero, isFacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            spriteBatch.End();

            if (debug)
            {
                spriteBatch.Begin();

                spriteBatch.DrawString(font, "Position: " + _position.X + ", " + _position.Y, new Vector2(50, 30), Color.White);
                spriteBatch.DrawString(font, "Action: " + _action.ToString(), new Vector2(50, 50), Color.White);
                spriteBatch.DrawString(font, "Grounded: " + grounded.ToString(), new Vector2(50, 70), Color.White);
                spriteBatch.DrawString(font, "Howl bonus: " + howlBonus.ToString(), new Vector2(50, 110), Color.White);
                spriteBatch.DrawString(font, "Fear gauge: " + _fear, new Vector2(50, 130), Color.White);
                spriteBatch.DrawString(font, "Afraid: " + _afraid, new Vector2(50, 150), Color.White);
                spriteBatch.DrawString(font, "Movement: " + movement, new Vector2(50, 170), Color.White);
                spriteBatch.DrawString(font, "Full moon: " + levelScreen.moonValue(), new Vector2(50, 190), Color.White);
                spriteBatch.DrawString(font, "Action: " + levelScreen.getComparisonValue() + " (" + levelScreen.getValue() + ")" +  " (" + levelScreen.strokeValue() + ")", new Vector2(50, 210), Color.White);

                if (lastSafeCollider != null)
                    spriteBatch.DrawString(font, "Friction: " + lastSafeCollider.friction, new Vector2(50, 90), Color.White);

                spriteBatch.End();
            }
        }

        # endregion


        private Vector2 CollisionSAT(Rectangle a, Rectangle b)
        {
            Vector2 projection = new Vector2();
            Vector2 centerToCenter = new Vector2(a.Center.X - b.Center.X, a.Center.Y - b.Center.Y);
            float overlapY = a.Height + b.Height + a.Width + b.Width;   // Initialize values with maximum values
            float overlapX = overlapY;

            if (Math.Abs(centerToCenter.Y) < a.Height / 2 + b.Height / 2)
                overlapY = (a.Height / 2 + b.Height / 2) - Math.Abs(centerToCenter.Y);

            if (Math.Abs(centerToCenter.X) < a.Width / 2 + b.Width / 2)
                overlapX = (a.Width / 2 + b.Width / 2) - Math.Abs(centerToCenter.X);

            if (overlapY <= overlapX)
                projection.Y = overlapY * (centerToCenter.Y < 0 ? -1 : 1);
            else
                projection.X = overlapX * (centerToCenter.X < 0 ? -1 : 1);

            return projection;
        }

        private void CheckCollisionSAT()
        {
            Rectangle characterCollider = animations[(int)_action].GetCollider();
            Rectangle character = new Rectangle(_position.X + characterCollider.X, _position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            _grounded = false;

            foreach (Screens.Level.Collider col in levelScreen.GetColliders())
            {
                Rectangle obstacle = col.GetBox();

                if (col.isActive && character.Intersects(obstacle))   // correct the position in case it collided
                {
                    if (col.isHarmful)
                    {
                        // check damage and everything
                        BackToLastSafeCollider(backToStart);

                        return;
                    }

                    Vector2 counterForce = CollisionSAT(character, obstacle);

                    if (counterForce != Vector2.Zero)
                    {
                        character = new Rectangle(character.X + (int)counterForce.X, character.Y + (int)counterForce.Y, characterCollider.Width, characterCollider.Height);

                        if (counterForce.Y < 0)
                        {
                            _grounded = true;
                            lastSafeCollider = col;
                        }

                        counterForce.X = Math.Abs(counterForce.X); counterForce.Y = Math.Abs(counterForce.Y);
                        Collide(Vector2.Normalize(counterForce));
                    }
                }
            }

            _position.X = character.X - characterCollider.X;
            _position.Y = character.Y - characterCollider.Y;

        }
        

        /// <summary>
        /// Controls everything about the animation and the physics associated
        /// </summary>
        /// <param name="input"></param>
        private void AnimationControl(InputHandler input)
        {
            bool dont_move = false;

            switch (_action)
            {
                case Action.Howl:
                    {
                        dont_move = true;
                        if (animations[(int)_action].isFinished)
                        {
                            StartHowlBonus();
                            ChangeAction(Action.Idle);
                        }
                    } break;
                case Action.Dig:
                    {
                        dont_move = true;
                        if (animations[(int)_action].isFinished)
                        {
                            levelScreen.Dig(lastSafeCollider);
                            ChangeAction(Action.Idle);
                        }
                    } break;
                case Action.Drink:
                    {
                        dont_move = true;
                        if (animations[(int)_action].isFinished)
                        {
                            _fear -= drinkReduceFear;
                            ChangeAction(Action.Idle);
                        }
                    } break;
                default:
                    break;
            }

            if (dont_move)
                return;

            if (action == Action.Fall && lastSafeCollider == null)
                return;

            bool moved = Move(input);

            if (_action != Action.Jump && !_grounded)
                ChangeAction(Action.Fall);

            switch (_action)
            {
                case Action.Idle:
                    {
                        if (moved)
                            ChangeAction(Action.Walk);
                        if (input.WasPressed(Input.A) && CanJump())
                            ChangeAction(Action.Jump);
                        else if (input.WasPressed(Input.X))
                            ChangeAction(Action.Dig);
                        else if (input.WasPressed(Input.Y) && lastSafeCollider.isWater)
                            ChangeAction(Action.Drink);
                        else if (levelScreen.getComparisonValue() == soundState.HOWLING || input.WasPressed(Input.B))
                        {
                            if (levelScreen.moonValue())
                                ChangeAction(Action.Howl);
                        }
                        else if (!moved)
                            movement.X = movement.X * lastSafeCollider.friction;
                    } break;

                case Action.Walk:
                    {
                        if (!moved)
                            ChangeAction(Action.Idle);
                        if (input.WasPressed(Input.A) && CanJump())
                            ChangeAction(Action.Jump);
                    } break;

                case Action.Jump:
                    {
                        if (input.Contains(Input.A))
                        {
                            jumpSpeed += jumpSpeedStep;

                            if (jumpSpeed >= maxJumpSpeed + (howlBonus ? jumpHowlBoost : 0) - (_afraid ? fearOnusJump : 0))
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

        
        /// <summary>
        /// Checks for collision between the current frame and the rectangles of the level this character is in (needs adjustments)
        /// </summary>
        private void CheckCollisions()
        {
            Rectangle characterCollider = animations[(int)_action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(_position.X + characterCollider.X, _position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            _grounded = false;

            foreach (Screens.Level.Collider col in levelScreen.GetColliders())
            {
                Rectangle rect = col.GetBox();

                if (col.isActive && characterColliderPositioned.Intersects(rect))   // correct the position in case it collided
                {
                    if (col.isHarmful)
                    {
                        // check damage and everything
                        BackToLastSafeCollider(backToStart);

                        return;
                    }

                    // test every possible collision
                    if (CollisionFromLeft(characterColliderPositioned, rect))
                    {
                        this._position = new Rectangle(_position.X - (characterColliderPositioned.Right - rect.Left), _position.Y, _position.Width, _position.Height);
                        //Collide(Vector2.UnitX);
                    }
                    else if (CollisionFromRight(characterColliderPositioned, rect))
                    {
                        this._position = new Rectangle(_position.X + (rect.Right - characterColliderPositioned.Left), _position.Y, _position.Width, _position.Height);
                        //Collide(Vector2.UnitX);
                    }
                    else if (CollisionFromAbove(characterColliderPositioned, rect))
                    {
                        // The flickering when jumping against a wall is due to the imperfections in the spritesheet, which means it will work just fine when we change the assets
                        this._position = new Rectangle(_position.X, _position.Y - (characterColliderPositioned.Bottom - rect.Top), _position.Width, _position.Height);
                        Collide(Vector2.UnitY);
                        lastSafeCollider = col;
                        _grounded = true;
                    }
                    else if (CollisionFromUnder(characterColliderPositioned, rect))
                    {
                        this._position = new Rectangle(_position.X, _position.Y + (rect.Bottom - characterColliderPositioned.Top), _position.Width, _position.Height);
                        Collide(Vector2.UnitY);
                        ChangeAction(Action.Fall);
                    }
                    characterColliderPositioned = new Rectangle(_position.X + characterCollider.X, _position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);
                }
            }

            if (!_grounded && _action != Action.Fall && _action != Action.Jump)
            {
                characterColliderPositioned = new Rectangle(_position.X + characterCollider.X,
                    _position.Y + characterCollider.Y + characterCollider.Height / 2, characterCollider.Width, characterCollider.Height);

                foreach (Screens.Level.Collider collider in levelScreen.GetColliders())
                    if (collider.GetBox().Intersects(characterColliderPositioned))
                        _grounded = true;
            }
        }

        #region old collision detection
        private bool CollisionDetectionFromAbove(Rectangle character, Rectangle obstacle)
        {
            Vector2 centerToTopLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Y - obstacle.Center.Y); // obstacle
            Vector2 centerToTopRight = new Vector2(obstacle.Right - obstacle.Center.X, obstacle.Y - obstacle.Center.Y); // obstacle
            Vector2 centerToCenter = new Vector2(character.Center.X - obstacle.Center.X, character.Center.Y - obstacle.Center.Y); // obstacle and character

            centerToTopLeft.Normalize();
            centerToTopRight.Normalize();
            centerToCenter.Normalize();

            return centerToCenter.X >= centerToTopLeft.X && centerToCenter.Y <= centerToTopLeft.Y
                && centerToCenter.X <= centerToTopRight.X && centerToCenter.Y <= centerToTopRight.Y;

            

            /*return character.Bottom > obstacle.Top && character.Top < obstacle.Top
                        && character.Center.Y < obstacle.Top
                        && character.Right > obstacle.Left && character.Left < obstacle.Right;*/
        }

        private bool CollisionDetectionFromUnder(Rectangle character, Rectangle obstacle)
        {
            Vector2 centerToBottomLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Bottom - obstacle.Center.Y);
            Vector2 centerToBottomRight = new Vector2(obstacle.Right - obstacle.Center.X, obstacle.Bottom - obstacle.Center.Y);
            Vector2 centerToCenter = new Vector2(character.Center.X - obstacle.Center.X, character.Center.Y - obstacle.Center.Y); // obstacle and character

            centerToBottomLeft.Normalize();
            centerToBottomRight.Normalize();
            centerToCenter.Normalize();

            return centerToCenter.X > centerToBottomLeft.X && centerToCenter.Y > centerToBottomLeft.Y
                && centerToCenter.X < centerToBottomRight.X && centerToCenter.Y > centerToBottomRight.Y;

            /*return character.Top < obstacle.Bottom && character.Bottom > obstacle.Bottom
                        && character.Center.Y > obstacle.Bottom
                        && character.Right > obstacle.Left && character.Left < obstacle.Right;*/
        }

        private bool CollisionDetectionFromLeft(Rectangle character, Rectangle obstacle)
        {
            Vector2 centerToTopLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Y - obstacle.Center.Y);
            Vector2 centerToBottomLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Bottom - obstacle.Center.Y);
            Vector2 centerToCenter = new Vector2(character.Center.X - obstacle.Center.X, character.Center.Y - obstacle.Center.Y); // obstacle and character

            centerToTopLeft.Normalize();
            centerToBottomLeft.Normalize();
            centerToCenter.Normalize();

            return centerToCenter.X < centerToTopLeft.X && centerToCenter.Y > centerToTopLeft.Y
                && centerToCenter.X <= centerToBottomLeft.X && centerToCenter.Y < centerToBottomLeft.Y;

            /*return character.Right > obstacle.Left && character.Left < obstacle.Left
                        && character.Center.X < obstacle.Left
                        && character.Bottom > obstacle.Top && character.Top < obstacle.Bottom;*/
        }

        private bool CollisionDetectionFromRight(Rectangle character, Rectangle obstacle)
        {
            Vector2 centerToTopRight = new Vector2(obstacle.Right - obstacle.Center.X, obstacle.Y - obstacle.Center.Y);
            Vector2 centerToBottomRight = new Vector2(obstacle.Right - obstacle.Center.X, obstacle.Bottom - obstacle.Center.Y);
            Vector2 centerToCenter = new Vector2(character.Center.X - obstacle.Center.X, character.Center.Y - obstacle.Center.Y); // obstacle and character

            centerToTopRight.Normalize();
            centerToBottomRight.Normalize();
            centerToCenter.Normalize();

            return centerToCenter.X > centerToTopRight.X && centerToCenter.Y > centerToTopRight.Y
                && centerToCenter.X >= centerToBottomRight.X && centerToCenter.Y < centerToBottomRight.Y;

            /*return character.Left < obstacle.Right && character.Right > obstacle.Right
                        && character.Center.X > obstacle.Right
                        && character.Bottom > obstacle.Top && character.Top < obstacle.Bottom;*/
        }
        #endregion

        #region new collision detection

        private bool CollisionFromAbove(Rectangle character, Rectangle obstacle)
        {
            Vector2 inCenterToTopLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Y - obstacle.Center.Y);
            Vector2 inCenterToTopRight = new Vector2(obstacle.Right - obstacle.Center.X, inCenterToTopLeft.Y);

            Vector2 outCenterToBottomLeft = new Vector2(character.X - obstacle.Center.X, character.Bottom - obstacle.Center.Y);
            Vector2 outCenterToBottomRight = new Vector2(character.Right - obstacle.Center.X, outCenterToBottomLeft.Y);
            Vector2 outCenterToCenter = new Vector2(character.Center.X - obstacle.Center.X, character.Bottom - obstacle.Center.Y);

            inCenterToTopLeft.Normalize();
            inCenterToTopRight.Normalize();
            outCenterToBottomLeft.Normalize();
            outCenterToBottomRight.Normalize();
            outCenterToCenter.Normalize();

            if (character.Bottom > obstacle.Center.Y)
                return false;

            if (outCenterToBottomLeft.X > inCenterToTopLeft.X &&
                outCenterToBottomLeft.X < inCenterToTopRight.X)
                return true;

            if (outCenterToBottomRight.X < inCenterToTopRight.X &&
                outCenterToBottomRight.X > inCenterToTopLeft.X)
                return true;

            if (outCenterToCenter.X > inCenterToTopLeft.X &&
                outCenterToCenter.X < inCenterToTopRight.X)
                return true;

            return false;
        }

        private bool CollisionFromUnder(Rectangle character, Rectangle obstacle)
        {
            Vector2 inCenterToBottomLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Bottom - obstacle.Center.Y);
            Vector2 inCenterToBottomRight = new Vector2(obstacle.Right - obstacle.Center.X, inCenterToBottomLeft.Y);

            Vector2 outCenterToTopLeft = new Vector2(character.X - obstacle.Center.X, character.Y - obstacle.Center.Y);
            Vector2 outCenterToTopRight = new Vector2(character.Right - obstacle.Center.X, outCenterToTopLeft.Y);
            Vector2 outCenterToCenter = new Vector2(character.Center.X - obstacle.Center.X, character.Top - obstacle.Center.Y);

            inCenterToBottomLeft.Normalize();
            inCenterToBottomRight.Normalize();
            outCenterToTopLeft.Normalize();
            outCenterToTopRight.Normalize();
            outCenterToCenter.Normalize();

            if (character.Y < obstacle.Center.Y)
                return false;

            if (outCenterToTopLeft.X > inCenterToBottomLeft.X &&
                outCenterToTopLeft.X < inCenterToBottomRight.X)
                return true;

            if (outCenterToTopRight.X < inCenterToBottomRight.X &&
                outCenterToTopRight.X > inCenterToBottomLeft.X)
                return true;

            if (outCenterToCenter.X > inCenterToBottomLeft.X &&
                outCenterToCenter.X < inCenterToBottomRight.X)
                return true;

            return false;
        }

        private bool CollisionFromLeft(Rectangle character, Rectangle obstacle)
        {
            Vector2 inCenterToTopLeft = new Vector2(obstacle.X - obstacle.Center.X, obstacle.Y - obstacle.Center.Y);
            Vector2 inCenterToBottomLeft = new Vector2(inCenterToTopLeft.X, obstacle.Bottom - obstacle.Center.Y);

            Vector2 outCenterToTopRight = new Vector2(character.Right - obstacle.Center.X, character.Y - obstacle.Center.Y);
            Vector2 outCenterToBottomRight = new Vector2(outCenterToTopRight.X, character.Bottom - obstacle.Center.Y);
            Vector2 outCenterToCenter = new Vector2(character.Right - obstacle.Center.X, character.Center.Y - obstacle.Center.Y);

            inCenterToTopLeft.Normalize();
            inCenterToBottomLeft.Normalize();
            outCenterToTopRight.Normalize();
            outCenterToBottomRight.Normalize();
            outCenterToCenter.Normalize();

            if (character.Right > obstacle.Center.X)
                return false;

            if (outCenterToTopRight.Y > inCenterToTopLeft.Y &&
                outCenterToTopRight.Y < inCenterToBottomLeft.Y)
                return true;

            if (outCenterToBottomRight.Y < inCenterToTopLeft.Y &&
                outCenterToBottomRight.Y > inCenterToBottomLeft.Y)
                return true;

            if (outCenterToCenter.Y > inCenterToTopLeft.Y &&
                outCenterToCenter.Y < inCenterToBottomLeft.Y)
                return true;

            return false;
        }

        private bool CollisionFromRight(Rectangle character, Rectangle obstacle)
        {
            Vector2 inCenterToTopRight = new Vector2(obstacle.Right - obstacle.Center.X, obstacle.Y - obstacle.Center.Y);
            Vector2 inCenterToBottomRight = new Vector2(inCenterToTopRight.X, obstacle.Bottom - obstacle.Center.Y);

            Vector2 outCenterToTopLeft = new Vector2(character.X - obstacle.Center.X, character.Y - obstacle.Center.Y);
            Vector2 outCenterToBottomLeft = new Vector2(outCenterToTopLeft.X, character.Bottom - obstacle.Center.Y);
            Vector2 outCenterToCenter = new Vector2(character.Left - obstacle.Center.X, character.Center.Y - obstacle.Center.Y);

            inCenterToTopRight.Normalize();
            inCenterToBottomRight.Normalize();
            outCenterToTopLeft.Normalize();
            outCenterToBottomLeft.Normalize();
            outCenterToCenter.Normalize();

            if (character.Left < obstacle.Center.X)
                return false;

            if (outCenterToTopLeft.Y > inCenterToTopRight.Y &&
                outCenterToTopLeft.Y < inCenterToBottomRight.Y)
                return true;

            if (outCenterToBottomLeft.Y > inCenterToTopRight.Y &&
                outCenterToBottomLeft.Y < inCenterToBottomRight.Y)
                return true;

            if (outCenterToCenter.Y > inCenterToTopRight.Y &&
                outCenterToCenter.Y < inCenterToBottomRight.Y)
                return true;

            return false;
        }

        #endregion

        private void BackToLastSafeCollider(bool backToStart)
        {
            if (backToStart)
            {
                _position.X = (int)levelScreen.GetStartPos().X;
                _position.Y = (int)levelScreen.GetStartPos().Y;
            }
            else
            {
                Rectangle last = lastSafeCollider.GetBox();

                _position.X = last.Center.X - _position.Width / 2;
                _position.Y = last.Top - positionHeight;
            }

            Collide(Vector2.Zero);
        }

        private void UpdateFear(Rectangle character)
        {
            int i = 0;
            List<Rectangle> fearZones = levelScreen.GetFearAreas();

            while (i < fearZones.Count)
            {
                if (character.Intersects(fearZones[i]))
                    _fear += 3 * _fearRegenerationRate * game.deltaTime;

                if (_fear >= fearThreshold)
                    _afraid = true;

                if (_fear > fearMaxValue)
                    _fear = fearMaxValue;

                i++;
            }

            if (howlBonus)
                _fear = 0;
            else
                _fear -= _fearRegenerationRate * game.deltaTime;

            if (_fear < fearThreshold)
                _afraid = false;

            if (_fear < 0)
                _fear = 0;
        }

        private void UpdateMoonTrigger(Rectangle character)
        {
            if (howlBonus)
                return;

            List<Rectangle> moonTriggers = levelScreen.GetMoonTriggers();
            bool moonActivated = false;

            int i = 0;
            while (!moonActivated && i < moonTriggers.Count)
            {
                if (character.Intersects(moonTriggers[i]))
                {
                    moonActivated = true;
                    levelScreen.TriggerMoon(i);
                }

                i++;
            }
        }

        private bool Move(InputHandler input)
        {
            bool hasMoved = false;

            if (input.Contains(Input.Left))
            {
                hasMoved = true;

                if (isFacingLeft)
                    movement += new Vector2(movementSpeed * input.getValues().XaxisLeft, 0);
                else
                    isFacingLeft = true;
            }
            else if (input.Contains(Input.Right))
            {
                hasMoved = true;

                if (!isFacingLeft)
                    movement += new Vector2(movementSpeed * input.getValues().XaxisLeft, 0);
                else
                    isFacingLeft = false;
            }

            return hasMoved;
        }

        public void ChangeAction(Action newAction)
        {
            _action = newAction;
            animations[(int)_action].Start();
        }

        private void StartHowlBonus()
        {
            if (howlBonus) return;

            howlBonus = true;
            howlBonusTimer = TimeSpan.Zero;
            howlBonusEnded = false;
        }

        private void LoadAnimationsFromFile(string path)
        {
            StreamReader file = new StreamReader(path);
            string line;
            string[] values;
            bool readingFrame = true;
            bool readingCollider = false;
            Rectangle rectangle;
            Action addingAnimation = Action.Idle;

            while (!file.EndOfStream)
            {
                line = file.ReadLine();

                if (line.StartsWith("#") || line == "")
                    continue;

                if (line.StartsWith("animation "))
                {
                    values = line.Split(new char[] { ' ' });

                    if (values.Length < 2)
                        continue;

                    addingAnimation = (Action)Enum.Parse(typeof(Action), values[1]);
                }

                if (line.StartsWith("frame"))
                {
                    readingFrame = true;
                    readingCollider = false;
                    continue;
                }

                if (line.StartsWith("collider"))
                {
                    readingFrame = false;
                    readingCollider = true;
                    continue;
                }

                values = line.Split(new char[] { ' ' });

                if (values.Length < 4)     // Ignore extra parameters, as long as there is the minimum
                    continue;

                rectangle = new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));

                if (readingFrame)
                    animations[(int)addingAnimation].InsertFrame(rectangle);
                else if (readingCollider)
                    animations[(int)addingAnimation].InsertFrameCollider(rectangle);
            }
        }

        # endregion


        # region Auxiliar Methods

        /// <summary>
        /// Checks whether or not there is room above Edon for him to perform a jump
        /// </summary>
        private bool CanJump()
        {
            bool canJump = true;

            Rectangle characterCollider = animations[(int)_action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(_position.X + characterCollider.X,
                _position.Y + characterCollider.Y - characterCollider.Height, characterCollider.Width, characterCollider.Height);

            foreach (Screens.Level.Collider collider in levelScreen.GetColliders())
                if (collider.GetBox().Intersects(characterColliderPositioned))
                    if (collider.isActive)
                        canJump = false;

            return canJump && _grounded;
        }

        # endregion
    }
}
