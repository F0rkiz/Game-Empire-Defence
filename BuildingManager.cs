using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Empire_Defence.Buildings;

namespace Empire_Defence
{
    public class BuildingManager
    {
        private Castle _castle;
        private List<Wall> _walls = new();
        private List<House> _houses = new();
        private List<Tower> _towers = new();

        private Texture2D _castleTexture, _wallTexture, _houseTexture, _towerTexture, _projectileTexture;
        private Texture2D _phantomCastle, _phantomWall, _phantomHouse, _phantomTower;

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

            _phantomCastle = content.Load<Texture2D>("phantom_castle");
            _phantomWall = content.Load<Texture2D>("phantom_wall");
            _phantomHouse = content.Load<Texture2D>("phantom_house");
            _phantomTower = content.Load<Texture2D>("phantom_tower");

            _buildSites.Add(new BuildingSite(new Vector2(800, 350 - _castleTexture.Height), BuildingType.Castle, _phantomCastle));
            _buildSites.Add(new BuildingSite(new Vector2(400, 350 - _wallTexture.Height), BuildingType.Wall, _phantomWall));
            _buildSites.Add(new BuildingSite(new Vector2(500, 350 - _houseTexture.Height), BuildingType.House, _phantomHouse));
            _buildSites.Add(new BuildingSite(new Vector2(550, 350 - _towerTexture.Height), BuildingType.Tower, _phantomTower));
        }

        public void Update(Player player) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            // 1. Стены (самые передние)
            foreach (var wall in _walls)
                if (wall.IsAlive)
                    wall.Draw(spriteBatch);

            // 2. Дома
            foreach (var house in _houses)
                if (house.IsAlive)
                    house.Draw(spriteBatch);

            // 3. Башни
            foreach (var tower in _towers)
                if (tower.IsAlive)
                    tower.Draw(spriteBatch);

            // 4. Замок (самый дальний)
            if (_castle != null && _castle.IsAlive)
                _castle.Draw(spriteBatch);

            // 5. Фантомные участки
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
        }
        public List<Tower> GetTowers()
        {
            return _towers;
        }



        public void TryBuildNearby(Player player)
        {
            foreach (var site in _buildSites)
            {
                if (site.IsBuilt || !site.IsVisible)
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