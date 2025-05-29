using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Core
{
    public class AnimatedSprite
    {
        public Texture2D Texture;
        public int FrameWidth, FrameHeight;
        public int FrameCount;
        public float FrameTime;
        public Vector2 Position;

        private float timer;
        private int currentFrame;

        public AnimatedSprite(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float frameTime)
        {
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
            FrameTime = frameTime;
            timer = 0f;
            currentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= FrameTime)
            {
                currentFrame = (currentFrame + 1) % FrameCount;
                timer = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, Position, sourceRect, Color.White);
        }
    }
}
