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
        private Building target;

        public Enemy(Vector2 spawnPos, Texture2D texture)
        {
            Texture = texture;
            Position = new Vector2(spawnPos.X, 350 - texture.Height); // стоим на земле
        }

        public void Update(GameTime gameTime, List<Building> potentialTargets)
        {
            if (!IsAlive) return;

            attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Найти новую цель, если старая уничтожена
            if (target == null || !target.IsAlive)
                target = FindNearestTarget(potentialTargets);

            if (target != null)
            {
                float distance = Vector2.Distance(Position, target.Position);

                if (distance > 10f)
                {
                    Vector2 dir = target.Position - Position;
                    dir.Normalize();
                    Position += dir * Speed;
                }
                else
                {
                    // Атака
                    if (attackTimer <= 0)
                    {
                        target.HP -= 10;
                        attackTimer = attackCooldown;
                    }
                }
            }
        }

        private Building FindNearestTarget(List<Building> targets)
        {
            float closest = float.MaxValue;
            Building nearest = null;

            foreach (var b in targets)
            {
                if (!b.IsAlive) continue;

                float dist = Vector2.Distance(Position, b.Position);
                if (dist < closest)
                {
                    closest = dist;
                    nearest = b;
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
