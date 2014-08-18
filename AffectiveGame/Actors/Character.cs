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
        private const float _fearRegenerationRate = 32;
        private const int fearOnusSpeed = 100;
        private const int fearOnusJump = 300;

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
            //animations.Add(new Animation(false)); // drink

            LoadAnimationsFromFile(Environment.CurrentDirectory + @"\EdonAnim.txt");

            /*
            // TO-DO: add frames and colliders via text file (inside Animation)
            animations[(int)Action.Idle].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half of image
            animations[(int)Action.Walk].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half
            animations[(int)Action.Jump].InsertFrame(new Rectangle(570, 0, 570, 690)); // 2nd half
            animations[(int)Action.Fall].InsertFrame(new Rectangle(570, 0, 570, 690)); // 2nd half
            animations[(int)Action.Howl].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half
            animations[(int)Action.Dig].InsertFrame(new Rectangle(0, 0, 570, 690)); // 1st half

            //animations[(int)Action.Idle].InsertFrameCollider(new Rectangle(19, 43, 74, 56)); // 94, 216, 370, 280 / 5
            animations[(int)Action.Idle].InsertFrameCollider(new Rectangle(17, 79, 86, 54)); // 85, 394, 432, 271 / 5
            animations[(int)Action.Walk].InsertFrameCollider(new Rectangle(17, 79, 86, 54));

            animations[(int)Action.Jump].InsertFrameCollider(new Rectangle(36, 13, 50, 122)); // 751 / 5 - 570 / 5, 65, 249, 609 / 5
            animations[(int)Action.Fall].InsertFrameCollider(new Rectangle(36, 13, 50, 122));
            animations[(int)Action.Howl].InsertFrameCollider(new Rectangle(17, 79, 86, 54));
            animations[(int)Action.Dig].InsertFrameCollider(new Rectangle(17, 79, 86, 54));*/
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

            CheckCollisions();
            
            UpdateFear();
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
                spriteBatch.DrawString(font, "Action: " + levelScreen.getComparisonValue() + " (" + levelScreen.getValue() + ")", new Vector2(50, 190), Color.White);

                if (lastSafeCollider != null)
                    spriteBatch.DrawString(font, "Friction: " + lastSafeCollider.friction, new Vector2(50, 90), Color.White);

                spriteBatch.End();
            }
        }

        # endregion

        

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
                            if (levelScreen.fullMoon || debug)
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
                        BackToLastSafeCollider();

                        return;
                    }

                    // test every possible collision
                    if (CollisionDetectionFromAbove(characterColliderPositioned, rect))
                    {
                        // The flickering when jumping against a wall is due to the imperfections in the spritesheet, which means it will work just fine when we change the assets
                        this._position = new Rectangle(_position.X, _position.Y - (characterColliderPositioned.Bottom - rect.Top), _position.Width, _position.Height);
                        Collide(Vector2.UnitY);
                        lastSafeCollider = col;
                        _grounded = true;
                    }
                    else if (CollisionDetectionFromUnder(characterColliderPositioned, rect))
                    {
                        this._position = new Rectangle(_position.X, _position.Y + (rect.Bottom - characterColliderPositioned.Top), _position.Width, _position.Height);
                        Collide(Vector2.UnitY);
                        ChangeAction(Action.Fall);
                    }
                    else if (CollisionDetectionFromLeft(characterColliderPositioned, rect))
                    {
                        this._position = new Rectangle(_position.X - (characterColliderPositioned.Right - rect.Left), _position.Y, _position.Width, _position.Height);
                        //Collide(Vector2.UnitX);
                    }
                    else if (CollisionDetectionFromRight(characterColliderPositioned, rect))
                    {
                        this._position = new Rectangle(_position.X + (rect.Right - characterColliderPositioned.Left), _position.Y, _position.Width, _position.Height);
                        //Collide(Vector2.UnitX);
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
                && centerToCenter.X >= centerToBottomRight.X && centerToCenter.Y <= centerToBottomRight.Y;

            /*return character.Left < obstacle.Right && character.Right > obstacle.Right
                        && character.Center.X > obstacle.Right
                        && character.Bottom > obstacle.Top && character.Top < obstacle.Bottom;*/
        }

        private void BackToLastSafeCollider()
        {
            Rectangle last = lastSafeCollider.GetBox();

            _position.X = last.Center.X - _position.Width / 2;
            _position.Y = last.Top - positionHeight;

            Collide(Vector2.Zero);
        }

        private void UpdateFear()
        {
            Rectangle characterCollider = animations[(int)_action].GetCollider();
            Rectangle characterColliderPositioned = new Rectangle(_position.X + characterCollider.X, _position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);

            int i = 0;
            List<Rectangle> fearZones = levelScreen.GetFearAreas();

            while (i < fearZones.Count)
            {
                if (characterColliderPositioned.Intersects(fearZones[i]))
                    _fear += 3 * _fearRegenerationRate * game.deltaTime;

                if (_fear >= fearThreshold)
                    _afraid = true;

                if (_fear > 255)
                    _fear = 255;

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
