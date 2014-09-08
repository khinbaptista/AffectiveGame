using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AffectiveGame.Actors;

namespace AffectiveGame.Screens.Level
{
    abstract class LevelScreen : GameScreen
    {
        protected struct Tunnel
        {
            public int entranceIndex { get; private set; }
            public List<int> coverIndexes { get; private set; }

            public Tunnel(int entrance)
            {
                this = new Tunnel();
                entranceIndex = entrance;
                coverIndexes = new List<int>();
            }

            public void AddCover(int coverIndex)
            {
                coverIndexes.Add(coverIndex);
            }

            public void AddCoverRange(IEnumerable<int> coverIndexes)
            {
                this.coverIndexes.AddRange(coverIndexes);
            }
        }

        protected readonly Vector2 gravitySpeed;
        protected Actors.Character Edon;
        protected List<Collider> environmentColliders;
        protected List<Rectangle> fearArea;
        protected List<Rectangle> moonTriggers;
        protected Rectangle endZone;
        protected Rectangle startZone;
        protected Vector2 startPosition;

        protected List<Tunnel> tunnels;

        protected Comparison.Manager soundControl;
        public Camera camera;
        public Actors.Moon moon;
        protected Texture2D background;

        public LevelScreen(GameMain game, GameScreen father, float gravitySpeed, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, state)
        {
            this.gravitySpeed = new Vector2(0, gravitySpeed);

            environmentColliders = new List<Collider>();
            fearArea = new List<Rectangle>();
            tunnels = new List<Tunnel>();
            moonTriggers = new List<Rectangle>();
            //LoadContent(game.Content);

            soundControl = new Comparison.Manager();
            soundControl.startProcessing();
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            background = content.Load<Texture2D>("Sky2");
            camera = new Camera(game.viewport, startPosition);

            moon = new Moon(game, this);
            moon.LoadContent(content);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Edon.Update(gameTime);

            moon.Update(gameTime);

            Vector2 edonPosition = Edon.position;
            camera.SmoothMove(new Vector2(edonPosition.X, Edon.grounded ? edonPosition.Y - game.viewport.Height / 4 : camera.position.Y));
            camera.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            camera.HandleInput(input);
            Edon.HandleInput(input);

            if (input.getStatus().Contains(Input.LeftBumper))
            {
                game.AddScreen(new MainMenuScreen(game, null));
                this.ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            
            if (screenState == ScreenState.Hidden)
                return;

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, game.viewport.Width, game.viewport.Height), Color.White);
            spriteBatch.End();

            moon.Draw(gameTime, spriteBatch);
            Edon.Draw(spriteBatch, gameTime);
        }

        public List<Collider> GetColliders() { return environmentColliders; }

        public List<Rectangle> GetFearAreas() { return fearArea; }

        public List<Rectangle> GetMoonTriggers() { return moonTriggers; }

        public Vector2 GetGravity() { return gravitySpeed; }

        public Vector2 GetStartPos() { return startPosition; }

        public Rectangle GetEndZone() { return endZone; }

        public void Dig(Collider collider)
        {
            if (!collider.isDiggable)
                return;

            int index = environmentColliders.IndexOf(collider);
            environmentColliders[index].Dig();

            foreach (Tunnel tunnel in tunnels)
                if (tunnel.entranceIndex == index)
                {
                    foreach (int cover in tunnel.coverIndexes)
                        environmentColliders[cover].Deactivate();
                }

        }

        public bool moonValue()
        {
            return moon.getMoonValue();
        }

        public void TriggerMoon(int index)
        {
            moon.startFullMoon();
            moonTriggers.RemoveAt(index);
        }

        /// <summary>
        /// Reads a file to load the colliders. Assumes the file contains one collider per line, values separated by ' ' (space).
        /// Both constructors are available: 4 values to describe the rectangle | 8 values, where "true" and "false" should be used.
        /// The 8-argument constructor is: the 4 values of the rectangle, booleans isHarmful, isDiggable, isWater, float friction
        /// </summary>
        /// <param name="filePath"></param>
        public void LevelFromFile(string path)
        {
            StreamReader file = new StreamReader(path);
            string text;
            string[] values;

            bool readingColliders = true;
            bool readingFearZones = false;
            bool readingTunnel = false;
            bool readingMoonTrigger = false;
            bool readingStartZone = false;
            bool readingEndZone = false;

            while (!file.EndOfStream)
            {
                text = file.ReadLine();

                if (text.StartsWith("#") || text == "")
                    continue;

                if (text.StartsWith("fear"))
                {
                    readingColliders = false;
                    readingFearZones = true;
                    readingTunnel = false;
                    readingMoonTrigger = false;
                    readingStartZone = false;
                    readingEndZone = false;
                    continue;
                }
                else if (text.StartsWith("collider"))
                {
                    readingFearZones = false;
                    readingColliders = true;
                    readingTunnel = false;
                    readingMoonTrigger = false;
                    readingStartZone = false;
                    readingEndZone = false;
                    continue;
                }
                else if (text.StartsWith("tunnel"))
                {
                    readingTunnel = true;
                    readingColliders = false;
                    readingFearZones = false;
                    readingMoonTrigger = false;
                    readingStartZone = false;
                    readingEndZone = false;
                    continue;
                }
                else if (text.StartsWith("moon"))
                {
                    readingMoonTrigger = true;
                    readingColliders = false;
                    readingFearZones = false;
                    readingTunnel = false;
                    readingStartZone = false;
                    readingEndZone = false;
                    continue;
                }
                else if (text.StartsWith("start"))
                {
                    readingMoonTrigger = false;
                    readingColliders = false;
                    readingFearZones = false;
                    readingTunnel = false;
                    readingStartZone = true;
                    readingEndZone = false;
                    continue;
                }
                else if (text.StartsWith("end"))
                {
                    readingMoonTrigger = false;
                    readingColliders = false;
                    readingFearZones = false;
                    readingTunnel = false;
                    readingStartZone = false;
                    readingEndZone = true;
                    continue;
                }

                values = text.Split(new char[] { ' ' });

                if (readingColliders)
                {
                    if (values.Length == 4)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3])));
                    else if (values.Length == 5)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4])));
                    else if (values.Length == 9)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]),
                                                    bool.Parse(values[4]), bool.Parse(values[5]), bool.Parse(values[6]), float.Parse(values[7], System.Globalization.CultureInfo.InvariantCulture), int.Parse(values[8])));
                }
                else if (readingFearZones)
                {
                    if (values.Length == 4)
                        fearArea.Add(new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3])));
                }
                else if (readingTunnel)
                {
                    Tunnel newTunnel = new Tunnel(int.Parse(values[0]));
                    
                    for (int i = 1; i < values.Length; i++)
                        newTunnel.AddCover(int.Parse(values[i]));

                    tunnels.Add(newTunnel);
                }
                else if (readingMoonTrigger)
                {
                    if (values.Length == 4)
                        moonTriggers.Add(new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3])));
                }
                else if (readingStartZone)
                {
                    if (values.Length == 4)
                        startZone = new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                }
                else if (readingEndZone)
                {
                    if (values.Length == 4)
                        endZone = new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                }
            }

            file.Close();
        }

        public override void ExitScreen()
        {
            soundControl.stopProcessing();

            base.ExitScreen();
        }

        public soundState getComparisonValue()
        {
            return soundControl.getActionValue();
        }

        public double getValue()
        {
            return soundControl.getValue();
        }

        public double strokeValue()
        {
            return soundControl.strokeValue();
        }

        /// <summary>
        /// Checks collision and updates position
        /// </summary>
        /// <param name="character">Current character collider</param>
        /// <returns>New grounded value</returns>
        public bool CheckCollision(Rectangle character, Rectangle characterCollider)
        {
            Rectangle colliderBox;
            bool grounded = false;

            foreach (Collider collider in environmentColliders)
            {
                colliderBox = collider.GetBox();

                if (collider.isActive && character.Intersects(colliderBox))
                {
                    if (collider.isHarmful)
                    {
                        // damage and teleport

                    }

                    if (character.Bottom > colliderBox.Top && character.Top < colliderBox.Top
                        && character.Center.Y < colliderBox.Top)
                    {
                        Edon.position = new Vector2(Edon.position.X, Edon.position.Y - (character.Bottom - colliderBox.Top));
                        Edon.Collide(Vector2.UnitY);
                        Edon.lastSafeCollider = collider;
                        grounded = true;
                    }
                    else if (character.Top < colliderBox.Bottom && character.Bottom > colliderBox.Bottom
                        && character.Center.Y > colliderBox.Bottom)
                    {
                        Edon.position = new Vector2(Edon.position.X, Edon.position.Y + (colliderBox.Bottom - character.Top));
                        Edon.Collide(Vector2.UnitY);
                        Edon.ChangeAction(Character.Action.Fall);
                    }
                    else if (character.Right > colliderBox.Left && character.Left < colliderBox.Left
                        && character.Center.X < colliderBox.Left)
                    {
                        Edon.position = new Vector2(Edon.position.X - (character.Right - colliderBox.Left));
                        Edon.Collide(Vector2.UnitX);
                    }
                    else if (character.Left < colliderBox.Right && character.Right > colliderBox.Right
                        && character.Center.X > colliderBox.Right)
                    {
                        Edon.position = new Vector2(Edon.position.X + (colliderBox.Right - character.Left));
                        Edon.Collide(Vector2.UnitX);
                    }
                }

                character = new Rectangle((int)Edon.position.X + characterCollider.X, (int)Edon.position.Y + characterCollider.Y, characterCollider.Width, characterCollider.Height);
            }

            return grounded;
        }
    }
}
