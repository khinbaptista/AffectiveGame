using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AffectiveGame.Screens
{
    /// <summary>
    /// This class merely shows a menu from where we can test various different screens (pause, pop up, and so on)
    /// </summary>
    class TestScreen : GameScreen
    {
        Menus.Menu testMenu;

        public TestScreen(GameMain game, GameScreen father, Viewport viewport, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, viewport, state)
        {
            transitionOnTime = TimeSpan.FromSeconds(0.5);
            transitionOffTime = TimeSpan.FromSeconds(0.1);

            testMenu = new Menus.Menu(viewport.Width / 4, (viewport.Height / 4) * 3, false, true, 1, 30);
            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            testMenu.LoadContent(content);
            testMenu.AddEntry("Pause screen");
            testMenu.AddEntry("Pop up window");
            testMenu.AddEntry("Back to main menu");
        }

        public override void HandleInput(InputHandler input)
        {
            if (screenState != ScreenState.Active)
                return;

            if (input.IsNewStatus(Input.Up))
                testMenu.MoveSelection(false);
            else if (input.IsNewStatus(Input.Down))
                testMenu.MoveSelection(true);
            else if (input.IsNewStatus(Input.A))
                MenuSelected();
        }

        private void MenuSelected()
        {
            if (testMenu.selectedEntry == 2)
            {
                father.Unhide();
                this.ExitScreen();
            }
            else if (testMenu.selectedEntry == 1)
            {
                game.AddScreen(new PopUpTestScreen(game, this, viewport, "Test pop up", "OK", "Cancel", new Rectangle(viewport.Width / 2 - 300, viewport.Height / 2 - 200, 600, 400)));
                this.ToggleUnderneath();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            testMenu.Draw(spriteBatch);
            
            base.Draw(gameTime, spriteBatch);
        }

    }
}
