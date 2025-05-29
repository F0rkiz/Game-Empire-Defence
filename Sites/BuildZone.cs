using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BuildZone
{
    public Vector2 Position;
    public string StructureType; 
    public int Cost;
    public bool IsBuilt;
    private Texture2D phantomTexture;
    private Texture2D builtTexture;

    public BuildZone(Vector2 position, string type, int cost, Texture2D phantom, Texture2D built)
    {
        Position = position;
        StructureType = type;
        Cost = cost;
        IsBuilt = false;
        phantomTexture = phantom;
        builtTexture = built;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var texture = IsBuilt ? builtTexture : phantomTexture;
        spriteBatch.Draw(texture, Position, Color.White * (IsBuilt ? 1f : 0.5f)); 
    }
}
