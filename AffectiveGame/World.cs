using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame
{
    class World
    {
        private int points;
        private float distance;
        private VertexPositionColor[] vertexArray;
        private short[] vertexIndexes;

        public World()
        {
            points = 100;
            distance = 5;
            vertexArray = new VertexPositionColor[points];
            vertexIndexes = new short[(points * 2) - 2];

            for (int x = 0; x > 10; x++)
                for (int z = 0; z < 10; z++)
                {
                    vertexArray[(x * 10) + z] = new VertexPositionColor(new Vector3(x * distance, -5, z * distance), Color.Black);
                }

            for (int i = 0; i < points - 1; i++)
            {
                vertexIndexes[i * 2] = (short)i;
                vertexIndexes[(i * 2) + 1] = (short)(i + 1);
            }
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertexArray, 0, points, vertexIndexes, 0, points - 1);
        }
    }
}
