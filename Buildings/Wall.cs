using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Buildings
{
    public class Wall : Building
    {
        public Wall(Vector2 position, Texture2D texture)
            : base(position, texture, 100) 
        {
        }

        public void Restore()
        {
            HP = MaxHP;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
                base.Draw(spriteBatch);
        }
    }
}
