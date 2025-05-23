using Empire_Defence.Buildings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Empire_Defence
{
    public class Enemy
    {
        public Vector2 Position;
        public Texture2D Texture;
        public float Speed = 1.5f;
        public int HP = 100;
        public bool IsAlive => HP > 0;

        private float attackCooldown = 1.0f;
        private float attackTimer = 0f;
        private IDamageable target;

        public Enemy(Vector2 spawnPos, Texture2D texture)
        {
            Texture = texture;
            Position = new Vector2(spawnPos.X, 800 - texture.Height);
        }

        public void Update(GameTime gameTime, List<IDamageable> targets)
        {
            if (!IsAlive) return;

            attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            target = FindNearestTarget(targets);

            if (target != null)
            {
                float distance = Vector2.Distance(Position, target.Position);
                float stopDistance = (Texture.Width + 32) / 2f;

                if (distance > stopDistance)
                {
                    Vector2 dir = target.Position - Position;
                    dir.Y = 0;
                    if (dir != Vector2.Zero)
                        dir.Normalize();
                    Position += dir * Speed;
                }
                else if (attackTimer <= 0)
                {
                    target.TakeDamage(10);
                    attackTimer = attackCooldown;
                }
            }
        }

        private IDamageable FindNearestTarget(List<IDamageable> targets)
        {
            IDamageable nearest = null;
            float closest = float.MaxValue;

            foreach (var t in targets)
            {
                if (!t.IsAlive) continue;

                float dist = Vector2.Distance(Position, t.Position);
                if (dist < closest)
                {
                    closest = dist;
                    nearest = t;
                }
            }

            return nearest;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
