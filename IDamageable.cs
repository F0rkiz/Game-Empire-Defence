using Microsoft.Xna.Framework;

namespace Empire_Defence
{
    public interface IDamageable
    {
        Vector2 Position { get; }
        void TakeDamage(int amount);
        bool IsAlive { get; }
    }
}
