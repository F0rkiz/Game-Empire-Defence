using Empire_Defence.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Buildings
{
    public abstract class Building : IDamageable
    {
        public Vector2 Position { get; protected set; }
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

        public void TakeDamage(int amount)
        {
            HP -= amount;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
                spriteBatch.Draw(Texture, Position, Color.White);
        }

        public bool IsAlive => HP > 0;
    }
}
