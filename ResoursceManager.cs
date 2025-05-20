using Microsoft.Xna.Framework;

namespace Empire_Defence
{
    public static class ResourceManager
    {
        private static int _gold = 200;

        public static int Gold => _gold;

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
