using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Empire_Defence.Buildings;

namespace Empire_Defence
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
