
using System.Collections.Generic;
using System.Linq;
using Empire_Defence.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Allies
{
    public class Warrior : Ally
    {
        private const float GroundY = 700f;

        private float attackCooldown = 1.0f;
        private float timer = 0f;

        public Warrior(Vector2 pos, Texture2D tex) : base(pos, tex, 80)
        {
            Damage = 15;
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

            var nearest = enemies.FirstOrDefault(e => e.IsAlive && Vector2.Distance(e.Position, Position) < 200);
            if (nearest != null)
            {
                Vector2 dir = nearest.Position - Position;
                dir.Y = 0;
                dir.Normalize();
                Position += dir * Speed;

                if (Vector2.Distance(Position, nearest.Position) < 40 && timer <= 0)
                {
                    nearest.HP -= Damage;
                    timer = attackCooldown;
                }
            }
        }

    }
}
