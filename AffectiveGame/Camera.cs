using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame
{
    class Camera
    {
        # region Attributes

        protected Viewport _viewport;
        protected Matrix _transform;
        protected Matrix _inverseTransform;
        protected Vector2 _position;
        protected float _zoom;

        protected float characterYpos;
        protected const float smoothSpeed = 512;

        #endregion



        # region Properties

        public Matrix transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public Matrix inverseTransform
        {
            get { return _inverseTransform; }
        }

        public Vector2 position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        # endregion



        # region Methods

        public Camera(Viewport viewport, Vector2 position)
        {
            _viewport = viewport;
            _transform = Matrix.Identity;
            _inverseTransform = Matrix.Identity;
            _position = position;
            _zoom = 1.0f;
        }

        public void HandleInput(InputHandler input)
        {
            //_zoom -= input.getValues().YaxisRight / 10;

            //_zoom = MathHelper.Clamp(_zoom, 0, 3);
        }

        public void Update(GameTime gameTime)
        {
            SmoothUpdate(gameTime);

            Vector2 origin = new Vector2(_viewport.Width / 2, _viewport.Height / 2);

            _transform = Matrix.Identity *
                        Matrix.CreateTranslation(-_position.X, -_position.Y, 0) *
                        Matrix.CreateRotationZ(0) *
                        Matrix.CreateScale(_zoom, _zoom, 0) *
                        Matrix.CreateTranslation(origin.X, origin.Y, 0);

            _inverseTransform = Matrix.Invert(_transform);
        }

        public void SmoothMove(Vector2 newPosition)
        {
            characterYpos = newPosition.Y;
            _position.X = newPosition.X;
        }

        private void SmoothUpdate(GameTime gameTime)
        {
            if (characterYpos == _position.Y)
                return;

            float finalSpeed = (float)(smoothSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (Math.Abs(_position.Y - characterYpos) <= finalSpeed)
                _position.Y = characterYpos;
            else if (_position.Y > characterYpos)
                _position.Y -= finalSpeed;
            else if (_position.Y < characterYpos)
                _position.Y += finalSpeed;
        }

        # endregion
    }
}
