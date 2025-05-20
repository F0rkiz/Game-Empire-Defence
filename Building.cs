using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Buildings
{
    public abstract class Building
    {
        public Vector2 Position;
        public Texture2D Texture;
        public int HP;
        public int MaxHP;

        public Building(Vector2 position, Texture2D texture, int maxHP)
        {
            Position = position;
            Texture = texture;
            MaxHP = maxHP;
            HP = maxHP;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

        public bool IsAlive => HP > 0;
    }
}
