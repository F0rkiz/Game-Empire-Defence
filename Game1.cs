using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Empire_Defence.Buildings;

namespace Empire_Defence
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D groundTile;
        private int screenWidth, screenHeight;

        private Player _player;
        private Texture2D _wallTexture;

        private List<Wall> _walls = new();
        private MouseState _previousMouse;

        private Matrix _cameraTransform;
        private Vector2 _cameraPosition;

        private SpriteFont _font;
        private BuildingManager _buildingManager;

        private List<Tower> _towers = new();
        private Texture2D _towerTexture;
        private Texture2D _projectileTexture;

        private List<Enemy> _enemies = new();
        private Texture2D _enemyTexture;

        private WaveManager _waveManager;
        private bool waveKeyPressed = false;
        private bool _waveKeyPressed = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _player = new Player { Position = new Vector2(100, 300) };
            _buildingManager = new BuildingManager();
            _waveManager = new WaveManager();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextureUtils.Initialize(GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            groundTile = Content.Load<Texture2D>("ground_tile");
            _wallTexture = Content.Load<Texture2D>("wall");
            _towerTexture = Content.Load<Texture2D>("tower");
            _projectileTexture = Content.Load<Texture2D>("projectile");
            _enemyTexture = Content.Load<Texture2D>("enemy");

            _player.LoadContent(Content);
            _font = Content.Load<SpriteFont>("DefaultFont");
            _buildingManager.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);
            _buildingManager.Update(_player);

            _cameraPosition = new Vector2(_player.Position.X - _graphics.PreferredBackBufferWidth / 2, 0);
            _cameraTransform = Matrix.CreateTranslation(new Vector3(-_cameraPosition, 0));

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                _buildingManager.TryBuildNearby(_player);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !_waveKeyPressed && !_waveManager.IsWaveActive)
            {
                _waveKeyPressed = true;

                _buildingManager.RestoreAllBuildings();

                _waveManager.StartNextWave();
                _buildingManager.ResetHouseGoldFlags();
                _buildingManager.GiveHouseIncome();
            }
            

            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                _waveKeyPressed = false;

            _waveManager.Update(gameTime, _enemies, _enemyTexture);

            var targets = GetAllBuildings();
            foreach (var enemy in _enemies)
                enemy.Update(gameTime, targets);

            foreach (var tower in _buildingManager.GetTowers())
            {
                tower.Update(gameTime, _enemies);

                foreach (var proj in tower.Projectiles)
                {
                    foreach (var enemy in _enemies)
                    {
                        if (Vector2.Distance(proj.Position, enemy.Position) < 20f && proj.IsActive && enemy.IsAlive)
                        {
                            enemy.HP -= 25;
                            proj.IsActive = false;
                        }
                    }
                }
            }



            _enemies.RemoveAll(e => !e.IsAlive);
            base.Update(gameTime);
        }

        private List<Empire_Defence.Buildings.Building> GetAllBuildings()
        {
            var buildings = new List<Empire_Defence.Buildings.Building>();
            if (_buildingManager.Castle != null && _buildingManager.Castle.HP > 0)
                buildings.Add(_buildingManager.Castle);
            buildings.AddRange(_buildingManager.GetAliveWalls());
            buildings.AddRange(_buildingManager.GetAliveHouses());
            return buildings;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(transformMatrix: _cameraTransform);

            int tileWidth = groundTile.Width;
            for (int x = -tileWidth; x < screenWidth + tileWidth; x += tileWidth)
                _spriteBatch.Draw(groundTile, new Vector2(x, 350), Color.White);

            _player.Draw(_spriteBatch);
            _buildingManager.Draw(_spriteBatch);

            foreach (var enemy in _enemies)
            {
                enemy.Draw(_spriteBatch);
                _spriteBatch.Draw(TextureUtils.WhitePixel, new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y - 10, enemy.HP, 4), Color.Red);
            }

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"Gold: {ResourceManager.Gold}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Wave: {_waveManager.CurrentWave}", new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(_font,
                _waveManager.IsWaveActive ? "Enemies incoming!" : "Press ENTER to start next wave",
                new Vector2(10, 70),
                _waveManager.IsWaveActive ? Color.Red : Color.LightGreen);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}