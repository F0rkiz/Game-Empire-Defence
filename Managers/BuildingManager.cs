
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Empire_Defence.Buildings;
using Microsoft.Xna.Framework.Input;
using System;
using Empire_Defence.Sites;
using Empire_Defence.Entities;
using Empire_Defence.Core;

namespace Empire_Defence.Managers
{

    public class BuildingManager
    {
        public Action<Tavern> OnTavernBuilt;

        private Castle _castle;
        private List<Wall> _walls = new();
        private List<House> _houses = new();
        private List<Tower> _towers = new();
        private List<Tavern> _taverns = new();

        private Texture2D _castleTexture, _wallTexture, _houseTexture, _towerTexture, _projectileTexture;
        private Texture2D _phantomCastle, _phantomWall, _phantomHouse, _phantomTower;
        private Texture2D _tavernArcherTexture, _tavernWarriorTexture;
        private Texture2D _phantomTavernArcher, _phantomTavernWarrior;

        private List<BuildingSite> _buildSites = new();

        public bool IsCastleBuilt { get; private set; }
        public Castle Castle => _castle;

        public void LoadContent(ContentManager content)
        {
            _castleTexture = content.Load<Texture2D>("castle");
            _wallTexture = content.Load<Texture2D>("wall");
            _houseTexture = content.Load<Texture2D>("house");
            _towerTexture = content.Load<Texture2D>("tower");
            _projectileTexture = content.Load<Texture2D>("projectile");

            _tavernArcherTexture = content.Load<Texture2D>("tavern_archer");
            _tavernWarriorTexture = content.Load<Texture2D>("tavern_warrior");


            _phantomCastle = content.Load<Texture2D>("phantom_castle");
            _phantomWall = content.Load<Texture2D>("phantom_wall");
            _phantomHouse = content.Load<Texture2D>("phantom_house");
            _phantomTower = content.Load<Texture2D>("phantom_tower");
            _phantomTavernArcher = content.Load<Texture2D>("phantom_tavern_archer");
            _phantomTavernWarrior = content.Load<Texture2D>("phantom_tavern_warrior");

            _buildSites.Add(new BuildingSite(new Vector2(1200, 815 - _castleTexture.Height), BuildingType.Castle, _phantomCastle));
            _buildSites.Add(new BuildingSite(new Vector2(400, 815 - _wallTexture.Height), BuildingType.Wall, _phantomWall));
            _buildSites.Add(new BuildingSite(new Vector2(1100, 815 - _houseTexture.Height), BuildingType.House, _phantomHouse));
            _buildSites.Add(new BuildingSite(new Vector2(1000, 815 - _houseTexture.Height), BuildingType.House, _phantomHouse));
            _buildSites.Add(new BuildingSite(new Vector2(1350, 815 - _houseTexture.Height), BuildingType.House, _phantomHouse));
            _buildSites.Add(new BuildingSite(new Vector2(650, 815 - _houseTexture.Height), BuildingType.House, _phantomHouse));
            _buildSites.Add(new BuildingSite(new Vector2(500, 815 - _towerTexture.Height), BuildingType.Tower, _phantomTower));
            _buildSites.Add(new BuildingSite(new Vector2(750, 815 - _tavernArcherTexture.Height), BuildingType.TavernArcher, _phantomTavernArcher));
            _buildSites.Add(new BuildingSite(new Vector2(900, 815 - _tavernWarriorTexture.Height), BuildingType.TavernWarrior, _phantomTavernWarrior));
        }

        public void Update(Player player) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var wall in _walls)
                if (wall.IsAlive)
                    wall.Draw(spriteBatch);

            foreach (var house in _houses)
                if (house.IsAlive)
                    house.Draw(spriteBatch);

            foreach (var tower in _towers)
                if (tower.IsAlive)
                    tower.Draw(spriteBatch);

            foreach (var tavern in _taverns)
                if (tavern.IsAlive)
                    tavern.Draw(spriteBatch);

            if (_castle != null && _castle.IsAlive)
                _castle.Draw(spriteBatch);

            foreach (var site in _buildSites)
                site.Draw(spriteBatch);
        }

        public void RestoreAllBuildings()
        {
            if (_castle != null && !_castle.IsAlive)
                _castle.HP = _castle.MaxHP;

            foreach (var wall in _walls)
                if (!wall.IsAlive)
                    wall.Restore();

            foreach (var house in _houses)
                if (!house.IsAlive)
                    house.Restore();

            foreach (var tower in _towers)
                if (!tower.IsAlive)
                    tower.Restore();

            foreach (var tavern in _taverns)
                if (!tavern.IsAlive)
                    tavern.Restore();
        }

        public List<Tower> GetTowers() => _towers;
        public List<Tavern> GetTaverns() => _taverns;

        public void TryBuildNearby(Player player, int mapWidth)
        {
            foreach (var site in _buildSites)
            {
                if (site.IsBuilt || !site.IsVisible)
                    continue;
                if (site.Position.X < 0 || site.Position.X > mapWidth)
                    continue;
                float distance = Vector2.Distance(player.Position, site.Position);
                if (distance < 80f)
                {
                    switch (site.Type)
                    {
                        case BuildingType.Castle:
                            if (ResourceManager.SpendGold(100))
                            {
                                _castle = new Castle(site.Position, _castleTexture);
                                IsCastleBuilt = true;
                                site.IsBuilt = true;
                            }
                            break;
                        case BuildingType.Wall:
                            if (IsCastleBuilt && ResourceManager.SpendGold(30))
                            {
                                _walls.Add(new Wall(site.Position, _wallTexture));
                                site.IsBuilt = true;
                            }
                            break;
                        case BuildingType.House:
                            if (IsCastleBuilt && ResourceManager.SpendGold(50))
                            {
                                _houses.Add(new House(site.Position, _houseTexture));
                                site.IsBuilt = true;
                            }
                            break;
                        case BuildingType.Tower:
                            if (IsCastleBuilt && ResourceManager.SpendGold(70))
                            {
                                _towers.Add(new Tower(site.Position, _towerTexture, _projectileTexture));
                                site.IsBuilt = true;
                            }
                            break;
                        case BuildingType.TavernArcher:
                            if (IsCastleBuilt && ResourceManager.SpendGold(40))
                            {
                                var tavern = new Tavern(site.Position, _tavernArcherTexture, TavernType.Archer);
                                _taverns.Add(tavern);
                                site.IsBuilt = true;

                                OnTavernBuilt?.Invoke(tavern);
                            }
                            break;
                        case BuildingType.TavernWarrior:
                            if (IsCastleBuilt && ResourceManager.SpendGold(40))
                            {
                                var tavern = new Tavern(site.Position, _tavernWarriorTexture, TavernType.Warrior);

                                _taverns.Add(tavern);
                                site.IsBuilt = true;

                                OnTavernBuilt?.Invoke(tavern);

                            }
                            break;
                    }

                    if (site.Type == BuildingType.Castle)
                        foreach (var s in _buildSites)
                            s.IsVisible = true;

                    break;
                }
            }
        }

        public void ResetHouseGoldFlags()
        {
            foreach (var house in _houses)
                house.ResetGoldFlag();
        }

        public void GiveHouseIncome()
        {
            foreach (var house in _houses)
                house.TryGiveGoldOnce();
        }

        public List<Wall> GetAliveWalls() => _walls.FindAll(w => w.HP > 0);
        public List<House> GetAliveHouses() => _houses.FindAll(h => h.HP > 0);
    }
}
