using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Empire_Defence.Sites
{
    public class BuildingSite
    {
        public Vector2 Position;
        public BuildingType Type;
        public bool IsBuilt;
        public bool IsVisible;

        private Texture2D _phantomTexture;

        public BuildingSite(Vector2 position, BuildingType type, Texture2D phantomTexture)
        {
            Position = position;
            Type = type;
            _phantomTexture = phantomTexture;
            IsBuilt = false;
            IsVisible = type == BuildingType.Castle; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsBuilt && IsVisible)
            {
                spriteBatch.Draw(_phantomTexture, Position, Color.White * 0.5f);
            }
        }

        public bool Contains(Vector2 worldPosition)
        {
            Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, 64, 64);
            return rect.Contains(worldPosition.ToPoint());
        }
    }

    public enum BuildingType
    {
        Castle,
        Wall,
        House,
        Tower,
        TavernArcher,
        TavernWarrior
    }
}
