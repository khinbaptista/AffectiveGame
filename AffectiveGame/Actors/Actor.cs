using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AffectiveGame.Actors
{
    abstract class Actor
    {
        List<Animation> animations;

        public Actor() { }

        public virtual void LoadContent(ContentManager content) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void HandleInput(InputHandler input) { }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
    }
}
