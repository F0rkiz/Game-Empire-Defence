using Empire_Defence.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Buildings
{
    public class House : Building
    {
        public int GoldPerWave = 10;
        public bool HasGivenGold = false;

        public House(Vector2 position, Texture2D texture)
            : base(position, texture, 75) { }

        public void TryGiveGoldOnce()
        {
            if (!HasGivenGold)
            {
                ResourceManager.AddGold(GoldPerWave);
                HasGivenGold = true;
            }
        }
        public void Restore()
        {
            HP = MaxHP;
        }

        public void ResetGoldFlag()
        {
            HasGivenGold = false;
        }
    }


}
