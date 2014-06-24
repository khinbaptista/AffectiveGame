using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AffectiveGame.Menus
{
    class MenuEntry
    {
        string text;

        SpriteFont font;
        int positionX;
        int positionY;
        float scale;
        bool isCentered;

        public MenuEntry(string text, SpriteFont font, int positionX, int positionY, float scale, bool isCentered)
        {
            this.text = text;
            this.font = font;
            this.scale = scale;
            this.isCentered = isCentered;

            this.positionX = isCentered ? positionX - (int)(font.MeasureString(text).X * scale / 2) : positionX;
            this.positionY = positionY;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, new Vector2(positionX, positionY), color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
