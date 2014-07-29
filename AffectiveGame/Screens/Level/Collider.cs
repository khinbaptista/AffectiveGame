﻿using System;
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
        private float _friction;
        private bool _isActive;

        # endregion

        #region Properties

        /// <summary>
        /// Inicates whether or not this collider causes damage to the character
        /// </summary>
        public bool isHarmful {
            get { return _isHarmful; }
            set { _isHarmful = value; } }

        /// <summary>
        /// Indicates whether or not this collider can be digged
        /// </summary>
        public bool isDiggable {
            get { return _isDiggable; }
            set { _isDiggable = value; } }

        /// <summary>
        /// Indicates whether or not this collider is water
        /// </summary>
        public bool isWater {
            get { return _isWater; }
            set { _isWater = value; } }

        /// <summary>
        /// In case the collider is harmful, this is how much damage it makes
        /// </summary>
        public float damage {
            get { return _isHarmful ? _damage : 0.0f; }
            set { _damage = value; } }

        /// <summary>
        /// Gets the friction for this collider - a value between 0 and 1, to be multiplied in the movement when idle
        /// </summary>
        public float friction
        {
            get { return _friction; }
        }

        /// <summary>
        /// Indicates if this collider is active (every collider is active by default, but can be digged, which disables them)
        /// </summary>
        public bool isActive
        {
            get { return _isActive; }
        }

        # endregion

        # region Methods

        public Collider(int posX, int posY, int posWidth, int posHeight)
        {
            box = new Rectangle(posX, posY, posWidth, posHeight);
            _isHarmful = false;
            _isDiggable = false;
            _isWater = false;
            _friction = 0.85f;
            _isActive = true;
        }

        public Collider(int posX, int posY, int posWidth, int posHeight, bool isHarmful, bool isDiggable, bool isWater, float friction)
        {
            box = new Rectangle(posX, posY, posWidth, posHeight);
            _isHarmful = isHarmful;
            _isDiggable = isDiggable;
            _isWater = isWater;
            _friction = friction;
            _isActive = true;
        }

        public Rectangle GetBox() { return box; }

        /// <summary>
        /// When a diggable collider is digged, it disappears
        /// </summary>
        public void Dig()
        {
            if (_isDiggable)
                _isActive = false;
        }

        /// <summary>
        /// Enables / Disables the collider. Shouldn't be used.
        /// </summary>
        /// <param name="state"></param>
        public void SetActive(bool state) { _isActive = state; }

        # endregion
    }
}
