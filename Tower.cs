using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Empire_Defence.Buildings;

namespace Empire_Defence
{
    public class Tower : Building
    {
        public Texture2D ProjectileTexture;
        public List<Projectile> Projectiles = new();

        private float fireCooldown = 1.5f;
        private float fireTimer = 0f;

        public Tower(Vector2 position, Texture2D texture, Texture2D projectileTexture)
            : base(position, texture, 75) // HP башни
        {
            ProjectileTexture = projectileTexture;

        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            fireTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            Enemy target = null;
            float closest = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float dist = Vector2.Distance(Position, enemy.Position);
                if (dist < 250 && dist < closest)
                {
                    closest = dist;
                    target = enemy;


                }
                if (target != null) ;


            }

            if (target != null && fireTimer <= 0)
            {
                fireTimer = fireCooldown;
                Projectiles.Add(new Projectile(Position, target.Position, ProjectileTexture));
            }

            foreach (var proj in Projectiles)
                proj.Update();
        }
        public void Restore()
        {
            HP = MaxHP;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                base.Draw(spriteBatch);
                foreach (var proj in Projectiles)
                    proj.Draw(spriteBatch);
            }
        }
    }
}
