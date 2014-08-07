using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;


namespace AffectiveGame.Screens
{
    /// <summary>
    /// This class merely shows a menu from where we can test various different screens (pause, pop up, and so on)
    /// </summary>
    class TestScreen : GameScreen
    {
        Menus.Menu testMenu;

        // Video stuff
        Video video;
        // VideoPlayer vPlayer;     // NOPE!: it should be in Microsoft.Xna.Framework.Media
        bool playVideo = false;

        public TestScreen(GameMain game, GameScreen father, ScreenState state = ScreenState.TransitionOn)
            : base(game, father, state)
        {
            transitionOnTime = TimeSpan.FromSeconds(0.5);
            transitionOffTime = TimeSpan.FromSeconds(0.1);

            testMenu = new Menus.Menu(game.viewport.Width / 4, (game.viewport.Height / 4) * 3, false, true, 1, 30);
            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            //video = content.Load<Video>("");

            testMenu.LoadContent(content);
            testMenu.AddEntry("Pause screen");
            testMenu.AddEntry("Pop up window");
            testMenu.AddEntry("Play unstoppable short video");
            testMenu.AddEntry("Back to main menu");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (playVideo)
            {
                playVideo = false;
                // play the damn video
            }
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (screenState != ScreenState.Active)
                return;

            if (input.WasPressed(Input.Up))
                testMenu.MoveSelection(false);
            else if (input.WasPressed(Input.Down))
                testMenu.MoveSelection(true);
            else if (input.WasPressed(Input.A))
                MenuSelected();
        }

        private void MenuSelected()
        {
            if (testMenu.selectedEntry == 3)
            {
                father.Unhide();
                this.ExitScreen();
            }
            else if (testMenu.selectedEntry == 2)
            {
                playVideo = true;
            }
            else if (testMenu.selectedEntry == 1)
                CreatePopup("This is a pop up", new Rectangle(game.viewport.Width / 2 - 400, game.viewport.Height / 2 - 300, 800, 600));
            else if (testMenu.selectedEntry == 0)
                CreatePauseScreen();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (screenState == ScreenState.Hidden)
                return;

            testMenu.Draw(spriteBatch);

            // draw the video (get last frame and draw it on screen)

            base.Draw(gameTime, spriteBatch);
        }

    }
}
