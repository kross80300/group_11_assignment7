using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_11_assignment7;

public class Asteroid
{
    Vector2 position;
    Vector2 velocity;
    Texture2D asteroidTexture;
    Rectangle boundingBox;
    int hitPoints = 3;
    float rotationAngle;
    float rotationSpeed;

    public void Update(GameTime gameTime)
    {
        position += velocity;
        
        rotationAngle += rotationSpeed;

        if (rotationAngle > MathHelper.TwoPi)
            rotationAngle -= MathHelper.TwoPi;

        boundingBox = new Rectangle(
            (int)(position.X - asteroidTexture.Width / 2f),
            (int)(position.Y - asteroidTexture.Height / 2f),
            asteroidTexture.Width,
            asteroidTexture.Height
        );
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            asteroidTexture,
            position,
            null,
            Color.White,
            rotationAngle,
            new Vector2(asteroidTexture.Width / 2f, asteroidTexture.Height / 2f),
            1.0f,
            SpriteEffects.None,
            0f
        );
    }

    public bool TakeDamage()
    {
        hitPoints--;
        if (hitPoints <= 0)
        {
            return true;
        }
        return false;

    }

    public Rectangle GetBoundingBox()
    {
        int width = asteroidTexture.Width;
        int height = asteroidTexture.Height;

        return new Rectangle(
            (int)(position.X - width / 2f),
            (int)(position.Y - height / 2f),
            width,
            height
        );
    }

    public bool IsOffScreen(int screenWidth, int screenHeight)
    {
        float halfWidth = asteroidTexture.Width / 2f;
        float halfHeight = asteroidTexture.Height / 2f;

        if (position.X + halfWidth < 0 ||
            position.X - halfWidth > screenWidth ||
            position.Y + halfHeight < 0 ||
            position.Y - halfHeight > screenHeight)
        {
            return true;
        }
        return false;
    }
    
}