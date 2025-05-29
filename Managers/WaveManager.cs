using Empire_Defence.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Empire_Defence.Managers
{
    public class WaveManager
    {
        public int CurrentWave { get; private set; } = 0;
        private float spawnInterval = 1.5f;
        private float spawnTimer = 0f;

        private int enemiesToSpawn = 0;
        private int enemiesSpawned = 0;

        public bool IsWaveActive { get; private set; } = false;

        public void StartNextWave()
        {
            CurrentWave++;
            enemiesToSpawn = 1 + CurrentWave * 2;
            enemiesSpawned = 0;
            spawnTimer = 0f;
            IsWaveActive = true;
        }

        public void Update(GameTime gameTime, List<Enemy> enemies, Texture2D enemyTexture, int mapWidth)
        {
            if (enemiesSpawned >= enemiesToSpawn && enemies.TrueForAll(e => !e.IsAlive))
            {
                IsWaveActive = false;
            }

            if (!IsWaveActive) return;

            spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spawnTimer <= 0f && enemiesSpawned < enemiesToSpawn)
            {
                spawnTimer = spawnInterval;

                float spawnX = -100;
                spawnX = MathHelper.Clamp(spawnX, 0, mapWidth - enemyTexture.Width);

                enemies.Add(new Enemy(new Vector2(spawnX, 800 - enemyTexture.Height), enemyTexture));
                enemiesSpawned++;
            }

            if (enemiesSpawned >= enemiesToSpawn && enemies.TrueForAll(e => !e.IsAlive))
            {
                IsWaveActive = false;
            }
        }
    }
}
