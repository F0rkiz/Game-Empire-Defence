using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Empire_Defence.Buildings
{
    public class Tavern : Building
    {
        public TavernType Type;
        public int Level = 1;

        public Tavern(Vector2 position, Texture2D texture, TavernType type)
            : base(position, texture, 100)
        {
            Type = type;
        }

        public int GetSpawnCount()
        {
            return Level switch
            {
                1 => 4,
                2 => 8,
                3 => 12,
                _ => 0
            };
        }

        public void Restore() => HP = MaxHP;
    }
    public enum TavernType
    {
        Archer,
        Warrior
    }
}
