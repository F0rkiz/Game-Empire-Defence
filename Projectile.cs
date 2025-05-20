using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence
{
    public class Projectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;
        public float Speed = 5f;
        public bool IsActive = true;

        public Projectile(Vector2 position, Vector2 target, Texture2D texture)
        {
            Position = position;
            Texture = texture;

            Vector2 direction = target - position;
            direction.Normalize();
            Velocity = direction * Speed;
        }

        public void Update()
        {
            Position += Velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
                spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
