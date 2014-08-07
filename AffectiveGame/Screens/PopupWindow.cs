using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AffectiveGame.Screens
{
    public class PopupWindow : GameScreen
    {
        #region Attributes

        private SpriteFont font;
        private Rectangle backgroundRect;
        private Texture2D background;

        private Texture2D GUI;
        private Rectangle GUI_buttonA = new Rectangle(455, 281, 67, 71);
        private Rectangle GUI_buttonB = new Rectangle(517, 213, 67, 71);

        private Vector2 messagePosition;
        private string message;
        private string textButtonA;
        private string textButtonB;

        public enum PopupReturn
        {
            Null,
            OK,
            Cancel
        }

        #endregion

        #region Methods

        public PopupWindow(GameMain game, GameScreen father, Rectangle position, string message)
            : base(game, father, ScreenState.Active)
        {
            transitionOnTime = TimeSpan.Zero;
            transitionOffTime = TimeSpan.Zero;

            backgroundRect = position;

            messagePosition = new Vector2(backgroundRect.X + 80, backgroundRect.Y + 50);
            this.message = message;
            textButtonA = "OK";
            textButtonB = "Cancel";

            LoadContent(game.Content);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            background = content.Load<Texture2D>("popupBackground");
            font = content.Load<SpriteFont>("tempFont");
            GUI = content.Load<Texture2D>("controllerButtons");
        }

        public void ChangeButtonText(string buttonA, string buttonB)
        {
            if (!String.IsNullOrEmpty(buttonA))
                textButtonA = buttonA;

            if (!String.IsNullOrEmpty(buttonB))
                textButtonB = buttonB;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (input.WasPressed(Input.A))
            {
                father.popupValue = PopupReturn.OK;
                FinishPopup();
            }
            if (input.WasPressed(Input.B))
            {
                father.popupValue = PopupReturn.Cancel;
                FinishPopup();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle positionA = new Rectangle(backgroundRect.X + 50, backgroundRect.Bottom - 90, 53, 56);
            Rectangle positionB = new Rectangle(backgroundRect.Right - 350, backgroundRect.Bottom - 90, 53, 56);
            int labelDistance = 20;

            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundRect, Color.White);
            spriteBatch.DrawString(font, message, messagePosition, Color.Black);
            
            spriteBatch.Draw(GUI, positionA, GUI_buttonA, Color.White);
            spriteBatch.DrawString(font, textButtonA, new Vector2(positionA.Right + labelDistance, positionA.Y + positionA.Height / 2 - 10), Color.Black);

            spriteBatch.Draw(GUI, positionB, GUI_buttonB, Color.White);
            spriteBatch.DrawString(font, textButtonB, new Vector2(positionB.Right + labelDistance, positionB.Y + positionB.Height / 2 - 10), Color.Black);
            
            spriteBatch.End();
            
            base.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Finishes this pop up managing the transition
        /// </summary>
        protected void FinishPopup()
        {
            if (father != null)
                father.ToggleUnderneath();

            this.KillScreen();
        }

        #endregion
    }
}
