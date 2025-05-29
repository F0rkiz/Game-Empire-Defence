using Empire_Defence.Allies;
using Empire_Defence.Buildings;
using Empire_Defence.Entities;
using Empire_Defence.Interfaces;
using Empire_Defence.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Empire_Defence.Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D groundTile;
        private int screenWidth, screenHeight;

        private List<Ally> _allies = new();
        private Texture2D _archerTexture;
        private Texture2D _warriorTexture;

        private Player _player;
        private Texture2D _wallTexture;
        Texture2D background;
        int mapWidth;
        int mapHeight;
        private List<Wall> _walls = new();
        private MouseState _previousMouse;

        private bool hasStartedOnce = false;

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

        private bool isGameOver = false;

        Rectangle btnContinue = new Rectangle(320, 170, 150, 30);
        Rectangle btnNewGame = new Rectangle(320, 210, 150, 30);
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
            _player = new Player { Position = new Vector2(700, 900) };
            _buildingManager = new BuildingManager();
            _waveManager = new WaveManager();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            TextureUtils.Initialize(GraphicsDevice);

            _buildingManager.OnTavernBuilt = SpawnInitialUnits;



            _spriteBatch = new SpriteBatch(GraphicsDevice);
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            groundTile = Content.Load<Texture2D>("ground_tile");
            _wallTexture = Content.Load<Texture2D>("wall");
            _towerTexture = Content.Load<Texture2D>("tower");
            _projectileTexture = Content.Load<Texture2D>("projectile");
            _enemyTexture = Content.Load<Texture2D>("warrior_goblin");

            _archerTexture = Content.Load<Texture2D>("archer");
            _warriorTexture = Content.Load<Texture2D>("warrior");

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

            if (Keyboard.GetState().IsKeyDown(Keys.D3)) // клавиша 3
            {
                foreach (var ally in _allies)
                    if (ally is Archer)
                        ally.FollowPlayer = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) // клавиша 2
            {
                foreach (var ally in _allies)
                    if (ally is Warrior)
                        ally.FollowPlayer = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                foreach (var ally in _allies)
                    ally.FollowPlayer = true;
                }

            if (Keyboard.GetState().IsKeyDown(Keys.F)) // зафиксировать в точке
            {
                foreach (var ally in _allies)
                    ally.FollowPlayer = false;
            }

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
                    if (hasStartedOnce && btnContinue.Contains(mousePos))
                    {
                        isGameStarted = true;
                    }
                    else if (btnNewGame.Contains(mousePos))
                    {
                        ResetGame();
                        isGameStarted = true;
                        hasStartedOnce = true;
                    }
                    else if (btnControls.Contains(mousePos))
                    {
                        isInInstructions = true;
                    }
                    else if (btnExit.Contains(mousePos))
                    {
                        Exit();
                    }
                }


                return;
            }

            if (state.IsKeyDown(Keys.Escape))
            {
                isGameStarted = false;
                isInInstructions = false;
                return;
            }
            if (_buildingManager.Castle != null && _buildingManager.Castle.HP <= 0)
            {
                isGameOver = true;
            }

            _enemies.RemoveAll(e => !e.IsAlive);

            if (!_waveManager.IsWaveActive && isNight)
            {
                isNight = false;
                background = backgroundDay;

                _buildingManager.RestoreAllBuildings();
                if (!_player.IsAlive)
                    _player.Respawn();
                SpawnAlliesFromTaverns();
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
            allTargets.AddRange(_allies);

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
            Dictionary<Enemy, int> plannedDamage = new();

            foreach (var ally in _allies)
            {
                ally.Update(gameTime, _enemies, _player);

                if (ally is Archer archer)
                {
                    foreach (var proj in archer.Projectiles)
                    {
                        foreach (var enemy in _enemies)
                        {
                            if (Vector2.Distance(proj.Position, enemy.Position) < 50f && proj.IsActive && enemy.IsAlive)
                            {
                                enemy.HP -= archer.Damage;
                                proj.IsActive = false;
                                Console.WriteLine("Попадание засчитано");
                            }
                        }
                    }

                    archer.Projectiles.RemoveAll(p => !p.IsActive);
                }
            }


            _enemies.RemoveAll(e => !e.IsAlive);
            base.Update(gameTime);
            if (isGameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    ResetGame();
                }

                return;
            }

            foreach (var ally in _allies)
                ally.Update(gameTime, _enemies, _player);



        }
        private void ResetGame()
        {
            _player = new Player();
            _player.LoadContent(Content);

            _waveManager = new WaveManager();
            _buildingManager = new BuildingManager();
            _buildingManager.LoadContent(Content);

            _enemies.Clear();
            ResourceManager.Gold = 2000;

            isNight = false;
            background = backgroundDay;

            isGameStarted = false;
            isInInstructions = false;
            isGameOver = false;
            hasStartedOnce = true;
            _buildingManager.OnTavernBuilt = SpawnInitialUnits;
            _allies.Clear();

        }



        private List<Buildings.Building> GetAllBuildings()
        {
            var buildings = new List<Buildings.Building>();
            if (_buildingManager.Castle != null && _buildingManager.Castle.HP > 0)
                buildings.Add(_buildingManager.Castle);
            buildings.AddRange(_buildingManager.GetAliveWalls());
            buildings.AddRange(_buildingManager.GetAliveHouses());
            return buildings;
        }
        private void SpawnAlliesFromTaverns()
        {
            foreach (var tavern in _buildingManager.GetTaverns())
            {
                int count = tavern.GetSpawnCount();
                for (int i = 0; i < count; i++)
                {
                    Vector2 spawnPos = tavern.Position + new Vector2(i * 10, 20);

                    if (tavern.Type == TavernType.Archer)
                        _allies.Add(new Archer(spawnPos, _archerTexture, _projectileTexture));

                    else
                        _allies.Add(new Warrior(spawnPos, _warriorTexture));
                }
            }
        }
        private void SpawnInitialUnits(Tavern tavern)
        {
            int count = tavern.GetSpawnCount();
            for (int i = 0; i < count; i++)
            {
                Vector2 spawn = tavern.Position + new Vector2(i * 10, 20);
                if (tavern.Type == TavernType.Archer)
                    _allies.Add(new Archer(spawn, _archerTexture, _projectileTexture));
                else
                    _allies.Add(new Warrior(spawn, _warriorTexture));
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            Console.WriteLine("Allies count: " + _allies.Count);

            GraphicsDevice.Clear(Color.Black);
            if (isGameOver)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, "ИГРА ОКОНЧЕНА", new Vector2(300, 200), Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(_font, "Нажмите ENTER, чтобы вернуться в меню", new Vector2(280, 300), Color.White);
                _spriteBatch.End();
                return;
            }
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
                    _spriteBatch.DrawString(_font, "Нажмите ESC, чтобы вернуться", new Vector2(300, 410), Color.Yellow);
                    _spriteBatch.DrawString(_font, "1 - Все союзники следуют за игроком", new Vector2(300, 280), Color.LightGray);
                    _spriteBatch.DrawString(_font, "2 - Только воины следуют за игроком", new Vector2(300, 310), Color.LightGray);
                    _spriteBatch.DrawString(_font, "3 - Только лучники следуют за игроком", new Vector2(300, 340), Color.LightGray);
                    _spriteBatch.DrawString(_font, "F - Все союзники остаются на месте", new Vector2(300, 370), Color.LightGray);
                }
                else
                {
                    if (hasStartedOnce)
                    {
                        _spriteBatch.Draw(TextureUtils.WhitePixel, btnContinue, new Color(0, 0, 0, 0));
                        _spriteBatch.DrawString(_font, "Продолжить", new Vector2(325, 180), Color.LightGreen);
                    }

                    _spriteBatch.Draw(TextureUtils.WhitePixel, btnNewGame, new Color(0, 0, 0, 0));
                    _spriteBatch.DrawString(_font, "Новая игра", new Vector2(325, 220), Color.LightBlue);

                    _spriteBatch.Draw(TextureUtils.WhitePixel, btnControls, new Color(0, 0, 0, 0));
                    _spriteBatch.DrawString(_font, "Управление", new Vector2(320, 260), Color.LightBlue);

                    _spriteBatch.Draw(TextureUtils.WhitePixel, btnExit, new Color(0, 0, 0, 0));
                    _spriteBatch.DrawString(_font, "Выйти", new Vector2(320, 300), Color.OrangeRed);


                    _spriteBatch.DrawString(_font, "EMPIRE DEFENCE", new Vector2(300, 120), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

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
            _buildingManager.Draw(_spriteBatch);

            foreach (var ally in _allies)
                ally.Draw(_spriteBatch);

            foreach (var enemy in _enemies)
                enemy.Draw(_spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"Gold: {ResourceManager.Gold}", new Vector2(20, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Wave: {_waveManager.CurrentWave}", new Vector2(20, 40), Color.White);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
