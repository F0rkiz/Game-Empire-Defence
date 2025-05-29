using Empire_Defence.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Empire_Defence.Allies
{
    public class Archer : Ally
    {
        private float attackCooldown = 2f;
        private float timer = 0f;
        private const float GroundY = 700f;

        public Texture2D ProjectileTexture;
        public List<Projectile> Projectiles = new();

        public Archer(Vector2 pos, Texture2D tex, Texture2D projectileTex) : base(pos, tex, 40)
        {
            ProjectileTexture = projectileTex;
            Damage = 10;
        }

        public override void Update(GameTime gameTime, List<Enemy> enemies, Player player)
        {
            if (!IsAlive) return;

            if (FollowPlayer)
            {
                Vector2 direction = player.Position - Position;
                if (direction.Length() > 40)
                {
                    direction.Normalize();
                    Position += direction * Speed;
                    Position = new Vector2(Position.X, GroundY);
                }
                return;
            }

            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            Dictionary<Enemy, int> plannedDamage = new();

            Enemy target = null;

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive || Vector2.Distance(enemy.Position, Position) > 300)
                    continue;

                int damageAlreadyPlanned = plannedDamage.TryGetValue(enemy, out int dmg) ? dmg : 0;

                if (enemy.HP - damageAlreadyPlanned > Damage)
                {
                    target = enemy;
                    break;
                }
            }

            if (target != null && timer <= 0)
            {
                timer = attackCooldown;

                Vector2 targetCenter = target.Position + new Vector2(target.Texture.Width / 2f, target.Texture.Height / 2f);
                Vector2 bowPosition = Position + new Vector2(0, Texture.Height / 2f);

                Projectiles.Add(new Projectile(bowPosition, targetCenter, ProjectileTexture));

                if (plannedDamage.ContainsKey(target))
                    plannedDamage[target] += Damage;
                else
                    plannedDamage[target] = Damage;
            }

            foreach (var proj in Projectiles)
            {
                if (proj.IsActive)
                    proj.Update();
            }

            Projectiles.RemoveAll(p => !p.IsActive);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (HP > 0 && Texture != null)
                spriteBatch.Draw(Texture, Position, Color.White);

            foreach (var proj in Projectiles)
            {
                if (proj.IsActive)
                    proj.Draw(spriteBatch);
            }
        }
    }
}
