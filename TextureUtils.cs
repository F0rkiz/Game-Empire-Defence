using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence
{
    public static class TextureUtils
    {
        public static Texture2D WhitePixel;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            WhitePixel = new Texture2D(graphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });
        }
    }
}
