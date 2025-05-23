using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        Texture2D background;
        int mapWidth;
        int mapHeight;
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
        private bool _waveKeyPressed = false;

        private bool isGameStarted = false;
        private bool isInInstructions = false;
        private bool isInMenu => !isGameStarted && !isInInstructions;

        Texture2D backgroundDay;
        Texture2D backgroundNight;
        bool isNight = false;

        Rectangle btnStart = new Rectangle(320, 210, 100, 30);
        Rectangle btnControls = new Rectangle(320, 250, 100, 30);
        Rectangle btnExit = new Rectangle(320, 290, 50, 30);


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _player = new Player { Position = new Vector2(700,900) };
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

            backgroundDay = Content.Load<Texture2D>("background_day");
            backgroundNight = Content.Load<Texture2D>("background_night");
            background = backgroundDay;
            mapWidth = background.Width;
            mapHeight = background.Height;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (!isGameStarted)
            {
                MouseState mouse = Mouse.GetState();
                Point mousePos = new Point(mouse.X, mouse.Y);

                if (isInInstructions)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) || mouse.LeftButton == ButtonState.Pressed)
                        isInInstructions = false;

                    return;
                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (btnStart.Contains(mousePos))
                        isGameStarted = true;

                    else if (btnControls.Contains(mousePos))
                        isInInstructions = true;

                    else if (btnExit.Contains(mousePos))
                        Exit();
                }

                return;
            }

            if (state.IsKeyDown(Keys.Escape))
            {
                isGameStarted = false;
                isInInstructions = false;
                return;
            }

            _enemies.RemoveAll(e => !e.IsAlive);

            if (!_waveManager.IsWaveActive && isNight)
            {
                isNight = false;
                background = backgroundDay;

                _buildingManager.RestoreAllBuildings();
                if (!_player.IsAlive)
                    _player.Respawn();
            }

            _player.Update(gameTime, background.Width);
            _buildingManager.Update(_player);

            _cameraPosition = Vector2.Zero;

            float zoom = 0.5f;
            _cameraTransform =
                Matrix.CreateTranslation(new Vector3(-_cameraPosition, 0)) *
                Matrix.CreateScale(zoom);

            if (state.IsKeyDown(Keys.Space))
            {
                _buildingManager.TryBuildNearby(_player, background.Width);
            }

            if (state.IsKeyDown(Keys.Enter) && !_waveKeyPressed && !_waveManager.IsWaveActive)
            {
                _waveKeyPressed = true;

                isNight = true;
                background = backgroundNight;

                _buildingManager.RestoreAllBuildings();
                _waveManager.StartNextWave();
                _buildingManager.ResetHouseGoldFlags();
                _buildingManager.GiveHouseIncome();
            }

            if (state.IsKeyUp(Keys.Enter))
                _waveKeyPressed = false;

            _waveManager.Update(gameTime, _enemies, _enemyTexture, background.Width);

            var targets = GetAllBuildings();
            List<IDamageable> allTargets = new();
            allTargets.Add(_player);
            allTargets.AddRange(targets);

            foreach (var enemy in _enemies)
                enemy.Update(gameTime, allTargets);

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
            GraphicsDevice.Clear(Color.Black);

            if (!isGameStarted)
            {
                _spriteBatch.Begin();

                if (isInInstructions)
                {
                    _spriteBatch.DrawString(_font, "УПРАВЛЕНИЕ", new Vector2(300, 100), Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    _spriteBatch.DrawString(_font, "WAD - движение", new Vector2(300, 160), Color.LightGray);
                    _spriteBatch.DrawString(_font, "Space - построить", new Vector2(300, 190), Color.LightGray);
                    _spriteBatch.DrawString(_font, "Enter - начать волну", new Vector2(300, 220), Color.LightGray);
                    _spriteBatch.DrawString(_font, "Esc - открыть меню", new Vector2(300, 250), Color.LightGray);
                    _spriteBatch.DrawString(_font, "Нажмите ESC, чтобы вернуться", new Vector2(300, 310), Color.Yellow);
                }
                else
                {
                    _spriteBatch.Draw(TextureUtils.WhitePixel, btnStart, new Color(0, 0, 0, 0));
                    _spriteBatch.Draw(TextureUtils.WhitePixel, btnControls, new Color(0, 0, 0, 0));
                    _spriteBatch.Draw(TextureUtils.WhitePixel, btnExit, new Color(0, 0, 0, 0));

                    _spriteBatch.DrawString(_font, "EMPIRE DEFENCE", new Vector2(300, 120), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                    _spriteBatch.DrawString(_font, "Начать игру", new Vector2(320, 220), Color.LightGreen);
                    _spriteBatch.DrawString(_font, "Управление", new Vector2(320, 260), Color.LightBlue);
                    _spriteBatch.DrawString(_font, "Выйти", new Vector2(320, 300), Color.OrangeRed);
                }

                _spriteBatch.End();
                return;
            }


            _spriteBatch.Begin(transformMatrix: _cameraTransform);
            for (int x = 0; x < mapWidth; x += background.Width * 2)
            {
                _spriteBatch.Draw(background, new Vector2(x, 0), Color.White);
            }

            _player.Draw(_spriteBatch);

            int hpBarWidth = 50;
            int hpBarHeight = 6;
            int hpFill = (int)(hpBarWidth * (_player.HP / (float)_player.MaxHP));
            Vector2 hpPosition = new Vector2(
                _player.Position.X + (_player.Texture.Width / 2) - (hpBarWidth / 2),
                _player.Position.Y - 10
            );
            _spriteBatch.Draw(TextureUtils.WhitePixel, new Rectangle((int)hpPosition.X, (int)hpPosition.Y, hpBarWidth, hpBarHeight), Color.DarkRed);
            _spriteBatch.Draw(TextureUtils.WhitePixel, new Rectangle((int)hpPosition.X, (int)hpPosition.Y, hpFill, hpBarHeight), Color.LimeGreen);

            _buildingManager.Draw(_spriteBatch);

            foreach (var enemy in _enemies)
                enemy.Draw(_spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"Gold: {ResourceManager.Gold}", new Vector2(20, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Wave: {_waveManager.CurrentWave}", new Vector2(20, 40), Color.White);
            //_spriteBatch.DrawString(_font,
            //    _waveManager.IsWaveActive ? "Enemies incoming!" : "Press ENTER to start next wave",
            //    new Vector2(10, 70),
            //    _waveManager.IsWaveActive ? Color.Red : Color.LightGreen);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
