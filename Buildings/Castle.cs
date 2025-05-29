using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Buildings
{
    public class Castle : Building
    {
        public Castle(Vector2 position, Texture2D texture)
            : base(position, texture, 300) 
        {
        }
        public void Restore()
        {
            HP = MaxHP;
        }
    }
}
