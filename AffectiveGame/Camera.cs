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
        Vector3 target;

        /// <summary>
        /// The up vector of the camera
        /// </summary>
        Vector3 up;

        /// <summary>
        /// The view matrix of this camera
        /// </summary>
        Matrix viewMatrix;

        bool isMovable;
        float movementSpeed;

        #region Constructors

        public Camera(Vector3 position, Vector3 target, Vector3 up)
        {
            this.position = position;
            this.target = target;
            this.up = up;

            Setup(false);
        }

        public Camera(float positionX, float positionY, float positionZ, Vector3 target, Vector3 up)
        {
            this.position = new Vector3(positionX, positionY, positionZ);
            this.target = target;
            this.up = up;

            Setup(false);
        }

        public Camera(float positionX, float positionY, float positionZ,
                        float targetX, float targetY, float targetZ, Vector3 up)
        {
            this.position = new Vector3(positionX, positionY, positionZ);
            this.target = new Vector3(targetX, targetY, targetZ);
            this.up = up;

            Setup(false);
        }

        #endregion

        public void Setup(bool isMovable, float movementSpeed = 0.0f)
        {
            this.isMovable = isMovable;
            this.movementSpeed = movementSpeed;

            CalculateViewMatrix();
        }

        public void CalculateViewMatrix()
        {
            viewMatrix = Matrix.CreateLookAt(position, target, up);
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

        /// <summary>
        /// Moves the camera towards the target
        /// </summary>
        public void Move()
        {
            position += target * movementSpeed;
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
            target += direction * movementSpeed;
            CalculateViewMatrix();
        }
    }
}
