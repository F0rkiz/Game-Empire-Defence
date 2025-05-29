using Microsoft.Xna.Framework;

namespace Empire_Defence.Core
{
    public static class ResourceManager
    {
        private static int _gold = 2000;

        public static int Gold
        {
            get => _gold;
            set => _gold = value;
        }


        public static void AddGold(int amount)
        {
            _gold += amount;
        }

        public static bool SpendGold(int amount)
        {
            if (_gold >= amount)
            {
                _gold -= amount;
                return true;
            }
            return false;
        }
    }
}
