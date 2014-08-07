using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Screens.Level
{
    abstract class LevelScreen : GameScreen
    {
        protected readonly Vector2 gravitySpeed;
        protected Actors.Character Edon;
        protected List<Collider> environmentColliders;
        protected List<Rectangle> fearArea;
        protected Vector2 startPosition;
        public Camera camera;
        
        public bool fullMoon { get; protected set; }
        
        protected Texture2D background;

        public LevelScreen(GameMain game, GameScreen father, float gravitySpeed, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, state)
        {
            this.gravitySpeed = new Vector2(0, gravitySpeed);

            //LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            background = content.Load<Texture2D>("Sky");
            camera = new Camera(game.viewport, startPosition);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Edon.Update(gameTime);

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

            Edon.Draw(spriteBatch, gameTime);
        }

        public ContentManager GetContentRef() { return game.Content; }

        public List<Collider> GetColliders() { return environmentColliders; }

        public List<Rectangle> GetFearAreas() { return fearArea; }

        public Vector2 GetGravity() { return gravitySpeed; }

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

            while (!file.EndOfStream)
            {
                text = file.ReadLine();

                if (text.StartsWith("#"))
                    continue;

                if (text.StartsWith("fear"))
                {
                    readingColliders = false;
                    readingFearZones = true;
                    continue;
                }
                else if (text.StartsWith("collider"))
                {
                    readingFearZones = false;
                    readingColliders = true;

                }

                values = text.Split(new char[] { ' ' });

                if (readingColliders)
                {
                    if (values.Length == 4)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3])));
                    else if (values.Length == 8)
                        environmentColliders.Add(new Collider(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]),
                                                    bool.Parse(values[4]), bool.Parse(values[5]), bool.Parse(values[6]), float.Parse(values[7])));
                }
                else if (readingFearZones)
                {
                    if (values.Length == 4)
                        fearArea.Add(new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3])));
                }

            }

            file.Close();
        }
    }
}
