using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using AffectiveGame.Actors;

namespace AffectiveGame.Screens.Level
{
    class LevelScreen : GameScreen
    {
        public const string LevelOneFile = @"\gapsize2.txt";
        SpriteFont font;
        bool debug = false;

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
        protected List<Actors.Fire> flame;
        protected List<Actors.TreeOnFire> fireTree;
        protected Rectangle endZone;
        protected Rectangle startZone;
        protected Vector2 startPosition;

        protected List<Tunnel> tunnels;

        protected Song backgroundSound;
        protected bool songstart = false;

        protected Comparison.Manager soundControl;
        public Camera camera;
        public Actors.Moon moon;
        protected Texture2D background;
        protected Texture2D grassGround;
        protected Texture2D stoneGround;
        protected Texture2D tunnelBG;
        protected Tree scenarioTrees;
        public string filepath { get; protected set; }

        public LevelScreen(GameMain game, GameScreen father, string levelFile, float gravitySpeed = 300, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, state)
        {
            this.gravitySpeed = new Vector2(0, gravitySpeed);

            environmentColliders = new List<Collider>();
            fearArea = new List<Rectangle>();
            tunnels = new List<Tunnel>();
            moonTriggers = new List<Rectangle>();
            flame = new List<Fire>();
            fireTree = new List<TreeOnFire>();
            filepath = levelFile;
            LoadContent(game.Content);

            backgroundSound = game.Content.Load<Song>("normalBack");

            soundControl = new Comparison.Manager();
            soundControl.startProcessing();
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("tempFont");

            background = content.Load<Texture2D>("Sky2");
            camera = new Camera(game.viewport, startPosition);

            moon = new Moon(game, this);
            moon.LoadContent(content);

            MediaPlayer.IsRepeating = true; 

            grassGround = content.Load<Texture2D>("stoneGround");
            stoneGround = content.Load<Texture2D>("stone");
            tunnelBG = content.Load<Texture2D>("Dirt-1");

            LevelFromFile(this.filepath);

            scenarioTrees = new Tree(game, this, environmentColliders);

            startPosition = new Vector2(startZone.X, startZone.Y);
            Edon = new Actors.Character(game, this, startPosition);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!songstart)
            {
                MediaPlayer.Play(backgroundSound);
                songstart = true;
            }  

            if (screenState != ScreenState.Active)
                return;

            Edon.Update(gameTime);
            moon.Update(gameTime);
            foreach (Fire fire in flame)
                fire.Update(gameTime);
            foreach (TreeOnFire burningTree in fireTree)
                burningTree.Update(gameTime);

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
            foreach (Fire fire in flame)
                fire.Draw(spriteBatch, gameTime);
            foreach(TreeOnFire burningTree in fireTree)
                burningTree.Draw(spriteBatch, gameTime);
            scenarioTrees.Draw(spriteBatch, gameTime);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, camera.transform);

            Texture2D usedSprite = grassGround;
            for (int i = 0; i < environmentColliders.Count; i++)
            {
                switch (environmentColliders[i].sprite)
                {
                    case -1:
                        usedSprite = tunnelBG;
                        break;
                    case 0:
                        usedSprite = blank;
                        break;
                    case 1:
                        usedSprite = grassGround;
                        break;
                    case 2:
                        usedSprite = stoneGround;
                        break;
                    default:
                        usedSprite = blank;
                        break;
                }
                if (environmentColliders[i].isActive || environmentColliders[i].isBG)
                {
                    if (environmentColliders[i].sprite != 0 || debug)
                        spriteBatch.Draw(usedSprite, environmentColliders[i].GetBox(), Color.White);
                }
                //spriteBatch.Draw(blank, environmentColliders[i].GetBox(), new Color(0.6f, 0.5f, 0.2f, 0.0f));
            }

            // debug
            if (debug)
            {
                foreach (Rectangle fearZone in fearArea)
                    spriteBatch.Draw(blank, fearZone, new Color(0.3f, 0, 0, 0.5f));

                foreach (Rectangle moonTrigger in moonTriggers)
                    spriteBatch.Draw(blank, moonTrigger, new Color(1, 1, 0.5f, 0.5f));

                spriteBatch.Draw(blank, startZone, new Color(0, 0.2f, 0.4f, 0.5f));
                spriteBatch.Draw(blank, endZone, new Color(0.4f, 0.1f, 0, 0.5f));
            }

            spriteBatch.End();

            Edon.Draw(spriteBatch, gameTime);

            calmBar(spriteBatch);
        }

        protected void calmBar(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Calm:", new Vector2(1050, 30), Color.White);
            spriteBatch.Draw(blank, new Rectangle(1048, 58, (int)(0.8 * 300) + 4, 24), Color.DarkSlateGray);
            spriteBatch.Draw(blank, new Rectangle(1050, 60, (int)(0.8 * 300), 20), Color.IndianRed);
            spriteBatch.Draw(blank, new Rectangle(1050, 60, (int)(0.8*(300 - (int)Edon.fear)), 20), Color.LightSteelBlue);
            spriteBatch.End();
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
                    {
                        environmentColliders[cover].Deactivate();
                        environmentColliders[cover].becomeBG();
                    }
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
            bool readingScenario = false;

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
                    readingScenario = false;
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
                    readingScenario = false;
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
                    readingScenario = false;
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
                    readingScenario = false;
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
                    readingScenario = false;
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
                    readingScenario = false;
                    continue;
                }
                else if (text.StartsWith("scenario"))
                {
                    readingMoonTrigger = false;
                    readingColliders = false;
                    readingFearZones = false;
                    readingTunnel = false;
                    readingStartZone = false;
                    readingEndZone = false;
                    readingScenario = true;
                    continue;
                }

                values = text.Split(new char[] { ' ' });

                if (readingColliders)
                {
                    if (values.Length == 4)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3])));
                    else if (values.Length == 6)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]), bool.Parse(values[5])));
                    else if (values.Length == 10)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]),
                                                    bool.Parse(values[4]), bool.Parse(values[5]), bool.Parse(values[6]), float.Parse(values[7], System.Globalization.CultureInfo.InvariantCulture), int.Parse(values[8]), bool.Parse(values[9])));
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
                else if (readingScenario)
                {
                    if (values.Length == 3)
                    {
                        switch (int.Parse(values[0]))
                        {
                            case 1:
                                flame.Add(new Fire(game, this, new Vector2(int.Parse(values[1]), int.Parse(values[2]))));
                                fearArea.Add(new Rectangle(int.Parse(values[1]), int.Parse(values[2]), flame[0].getDimensions().Width, flame[0].getDimensions().Height));
                                break;
                            case 2:
                                fireTree.Add(new TreeOnFire(game, this, new Vector2(int.Parse(values[1]), int.Parse(values[2]))));
                                fearArea.Add(new Rectangle(int.Parse(values[1]), int.Parse(values[2]), fireTree[0].getDimensions().Width, fireTree[0].getDimensions().Height));
                                break;
                        }
                    }
                }

            }

            file.Close();
        }

        public override void ExitScreen()
        {
            soundControl.stopProcessing();
            MediaPlayer.Stop();
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
