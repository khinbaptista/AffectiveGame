using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AffectiveGame.Menus
{
    class Menu
    {
        #region Attributes

        protected List<MenuEntry> entries;
        protected SpriteFont font;
        protected bool isCentered;
        protected bool isLoop;

        protected int positionX;
        protected int positionY;
        protected int scale;
        protected int spacing;

        protected Color selectedEntryColor;
        protected Color notSelectedEntryColor;

        public int selectedEntry { get; private set; }

        #endregion

        #region Methods

        public Menu(int positionX, int positionY, bool isCentered, bool isLoop, int fontScale = 1, int spacing = 30)
        {
            entries = new List<MenuEntry>();

            this.positionX = positionX;
            this.positionY = positionY;
            this.isCentered = isCentered;
            this.isLoop = isLoop;
            this.scale = fontScale;
            this.spacing = spacing;

            selectedEntryColor = Color.SteelBlue;
            notSelectedEntryColor = Color.White;
        }

        public void LoadContent(ContentManager content, Viewport viewport)
        {
            font = content.Load<SpriteFont>("tempFont");
        }

        public void AddEntry(string text)
        {
            entries.Add(new MenuEntry(text, font, positionX, positionY + entries.Count * scale * spacing, scale, isCentered));
        }

        public int CountEntries()
        {
            return entries.Count;
        }

        public void MoveSelection(bool forward = true)
        {
            if (forward)
            {
                selectedEntry++;

                if (selectedEntry >= entries.Count)
                    selectedEntry = isLoop ? 0 : entries.Count - 1;
            }
            else
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = isLoop ? entries.Count - 1 : 0;
            }
        }

        public bool SetSelection(int index = 0)
        {
            if (index >= 0 && index < entries.Count)
            {
                selectedEntry = index;
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                Color color = (i == selectedEntry) ? selectedEntryColor : notSelectedEntryColor;
                entries[i].Draw(spriteBatch, color);
            }
        }

        public void ResetEntries()
        {
            entries = new List<MenuEntry>();
        }

        #endregion
    }
}
