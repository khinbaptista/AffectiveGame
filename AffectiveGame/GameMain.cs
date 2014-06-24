#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace AffectiveGame
{
    /// <summary>
    /// Manages the game itself
    /// </summary>
    public class GameMain : Game
    {
        #region Attributes

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// The input manager of this game; any classes interested may access input through this
        /// </summary>
        InputHandler input;

        /// <summary>
        /// List of currently existing screens
        /// </summary>
        List<Screens.GameScreen> screens;

        #endregion

        #region Methods

        public GameMain()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            input = new InputHandler();
            screens = new List<Screens.GameScreen>();

            // screens.Add(new Screens.GameScreen(this, GraphicsDevice.Viewport));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (Screens.GameScreen screen in screens)
                screen.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape) || screens.Count == 0)
                Exit();

            input.Update();

            int i = 0;
            while (i < screens.Count)
            {
                if (screens[i].delete)
                {
                    screens.RemoveAt(i);
                    continue;
                }

                screens[i].Update(gameTime);
                i++;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            foreach (Screens.GameScreen screen in screens)
                screen.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        #endregion

        #region Auxiliar methods



        #endregion

    }
}
