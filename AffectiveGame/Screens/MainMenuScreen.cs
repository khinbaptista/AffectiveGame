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

        public MainMenuScreen(GameMain game, Viewport viewport, ScreenState state = ScreenState.TransitionOn)
            : base(game, viewport, state)
        {
            this.transitionOnTime = TimeSpan.FromSeconds(0.5);
            this.transitionOffTime = TimeSpan.FromSeconds(0.5);

            mainMenu = new Menus.Menu(viewport.Width / 2, viewport.Height / 2 + 50, true, false, 1, 30);
        }

        public override void LoadContent(ContentManager content)
        {
            mainMenu.LoadContent(content);
            mainMenu.AddEntry("Start game");
            mainMenu.AddEntry("Exit");

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(InputHandler input)
        {
            if (input.IsNewStatus(Input.Up))
                mainMenu.MoveSelection(false);
            else if (input.IsNewStatus(Input.Down))
                mainMenu.MoveSelection(true);
            else if (input.IsNewStatus(Input.A))
                //MenuSelected();
                if (mainMenu.selectedEntry == 0)
                {
                    // game.AddScreen(new GameScreen(game, viewport));
                }
                else if (mainMenu.selectedEntry == 1)
                    this.ExitScreen();
        }

        private void MenuSelected()
        {
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mainMenu.Draw(spriteBatch);

            base.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
