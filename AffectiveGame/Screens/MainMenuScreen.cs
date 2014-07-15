using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AffectiveGame.Screens
{
    class MainMenuScreen : GameScreen
    {
        # region Attributes

        Menus.Menu mainMenu;

        #endregion

        #region Methods

        public MainMenuScreen(GameMain game, GameScreen father, Viewport viewport, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {
            this.transitionOnTime = TimeSpan.FromSeconds(0.5);
            this.transitionOffTime = TimeSpan.FromSeconds(0.1);

            mainMenu = new Menus.Menu(viewport.Width / 2, viewport.Height / 2 + 150, true, false, 1, 30);
            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            mainMenu.LoadContent(content);
            mainMenu.AddEntry("Start game");
            mainMenu.AddEntry("Screen tests");
            mainMenu.AddEntry("Exit");

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (screenState != ScreenState.Active)
                return;

            if (input.IsNewStatus(Input.Up))
                mainMenu.MoveSelection(false);
            else if (input.IsNewStatus(Input.Down))
                mainMenu.MoveSelection(true);
            else if (input.IsNewStatus(Input.A))
                MenuSelected();
        }

        private void MenuSelected()
        {
            if (mainMenu.selectedEntry == 0)
            {
                game.AddScreen(new Level.LevelScreen(game, null, viewport));
                this.ExitScreen();
            }
            else if (mainMenu.selectedEntry == 1)
            {
                this.Hide();
                game.AddScreen(new TestScreen(game, this, viewport));
            }
            else if (mainMenu.selectedEntry == 2)
                this.ExitScreen();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (screenState == ScreenState.Hidden)
                return;

            mainMenu.Draw(spriteBatch);

            base.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
