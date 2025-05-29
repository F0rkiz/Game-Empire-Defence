using Empire_Defence.Entities;
using Empire_Defence.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Empire_Defence.Allies
{
    public abstract class Ally : IDamageable
    {
        public Vector2 Position { get; protected set; }
        public Texture2D Texture;
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Damage;
        public bool FollowPlayer = false;
        public float Speed = 2f;

        public bool IsAlive => HP > 0;

        public void TakeDamage(int amount)
        {
            HP -= amount;
        }

        public Ally(Vector2 pos, Texture2D tex, int hp)
        {
            Position = pos;
            Texture = tex;
            HP = MaxHP = hp;
        }

        public abstract void Update(GameTime gameTime, List<Enemy> enemies, Player player);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (HP > 0 && Texture != null)
                spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
