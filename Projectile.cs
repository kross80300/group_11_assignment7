using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace project7
{
    public class Projectile
    {
        public Vector2 position;
        private Vector2 velocity;
        private Texture2D texture;
        private float scale = 0.5f;

        public Projectile(Texture2D texture, Vector2 position, Vector2 velocity)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
        }

        public void Update(GameTime time)
        {
            position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,position,null,Color.White,0f,Vector2.Zero,scale,SpriteEffects.None,0f);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X,(int)position.Y,(int)(texture.Width * scale),(int)(texture.Height * scale));
        }

        public bool IsOffScreen(int height)
        {
            return position.Y < -20 || position.Y > height + 20;
        }
    }
}