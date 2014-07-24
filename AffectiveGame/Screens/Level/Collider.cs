using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AffectiveGame.Screens.Level
{
    class Collider
    {
        # region Attributes

        private Rectangle box;
        private bool _isHarmful;
        private bool _isDiggable;
        private bool _isWater;
        private float _damage;

        # endregion

        #region Properties

        public bool isHarmful {
            get { return _isHarmful; }
            set { _isHarmful = value; } }
        public bool isDiggable {
            get { return _isDiggable; }
            set { _isDiggable = value; } }
        public bool isWater {
            get { return _isWater; }
            set { _isWater = value; } }
        public float damage {
            get { return _isHarmful ? _damage : 0.0f; }
            set { _damage = value; } }

        # endregion

        # region Methods

        public Collider(int posX, int posY, int posWidth, int posHeight)
        {
            box = new Rectangle(posX, posY, posWidth, posHeight);
            _isHarmful = false;
            _isDiggable = false;
            _isWater = false;
        }

        public Collider(int posX, int posY, int posWidth, int posHeight, bool isHarmful, bool isDiggable, bool isWater)
        {
            box = new Rectangle(posX, posY, posWidth, posHeight);
            _isHarmful = isHarmful;
            _isDiggable = isDiggable;
            _isWater = isWater;
        }

        public Rectangle GetBox() { return box; }

        # endregion
    }
}
