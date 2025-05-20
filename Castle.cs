using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Empire_Defence.Buildings;

namespace Empire_Defence
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
