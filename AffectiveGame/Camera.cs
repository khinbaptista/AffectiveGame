using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AffectiveGame
{
    class Camera
    {
        /// <summary>
        /// The position of the camera
        /// </summary>
        Vector3 position;

        /// <summary>
        /// The target (direction) of the camera
        /// </summary>
        Vector3 direction;

        /// <summary>
        /// The up vector of the camera
        /// </summary>
        Vector3 up;

        /// <summary>
        /// The view matrix of this camera
        /// </summary>
        Matrix viewMatrix;

        /// <summary>
        /// The projection matrix of this camera
        /// </summary>
        Matrix projectionMatrix;

        bool isMovable;
        float movementSpeed;

        #region Constructors

        public Camera(Vector3 position, Vector3 target, Vector3 up)
        {
            this.position = position;
            this.direction = target;
            this.up = up;

            //Setup(false);
        }

        public Camera(float positionX, float positionY, float positionZ, Vector3 target, Vector3 up)
        {
            this.position = new Vector3(positionX, positionY, positionZ);
            this.direction = target;
            this.up = up;

            //Setup(false);
        }

        public Camera(float positionX, float positionY, float positionZ,
                        float targetX, float targetY, float targetZ, Vector3 up)
        {
            this.position = new Vector3(positionX, positionY, positionZ);
            this.direction = new Vector3(targetX, targetY, targetZ);
            this.up = up;

            //Setup(false);
        }

        #endregion

        public void Setup(float aspectRatio, bool isMovable, float movementSpeed = 0.0f)
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView((float)(Math.PI / 2), aspectRatio, 0.1f, 100);

            this.isMovable = isMovable;
            this.movementSpeed = movementSpeed;

            //CalculateViewMatrix();
        }

        public void CalculateViewMatrix()
        {
            viewMatrix = Matrix.CreateLookAt(position, direction, up);
        }

        public void SetPosition(Vector3 newPosition)
        {
            this.position = newPosition;
        }

        public void SetPosition(float x, float y, float z)
        {
            this.position = new Vector3(x, y, z);
        }

        public void SetMovementSpeed(float speed)
        {
            this.movementSpeed = speed;
        }

        public void HandleInput(InputHandler input)
        {
            if (input.Contains(Input.Up))
                Move();
            if (input.Contains(Input.Down))
                Move(-direction);
        }

        /// <summary>
        /// Moves the camera towards the target
        /// </summary>
        public void Move()
        {
            position += direction * movementSpeed;
        }

        /// <summary>
        /// Moves the camera
        /// </summary>
        /// <param name="direction">Direction of movement</param>
        public void Move(Vector3 direction)
        {
            if (!isMovable)
                return;

            position += direction * movementSpeed;
            //this.direction += direction * movementSpeed;
            CalculateViewMatrix();
        }
    }
}
