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
        protected Vector2 _pos;
        protected float _zoom;

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
            get { return _pos; }
            set { _pos = value; }
        }

        public float zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        # endregion

        # region Methods

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            _transform = Matrix.Identity;
            _inverseTransform = Matrix.Identity;
            _pos = Vector2.Zero;
            _zoom = 1.0f;
        }

        public void HandleInput(InputHandler input)
        {
            _zoom -= input.getValues().YaxisRight / 10;
        }

        public void Update()
        {
            Vector2 origin = new Vector2(_viewport.Width / 2, _viewport.Height / 2);

            _transform = Matrix.Identity *
                        Matrix.CreateTranslation(-_pos.X, -_pos.Y, 0) *
                        Matrix.CreateRotationZ(0) *
                        Matrix.CreateScale(_zoom, _zoom, 0) *
                        Matrix.CreateTranslation(origin.X, origin.Y, 0);

            _inverseTransform = Matrix.Invert(_transform);
        }

        # endregion
    }
}
