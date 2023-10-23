using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP;

public abstract class PowerUp
{
    public BoundingBox BoundingBox { get; protected set; }

    protected PowerUp(BoundingBox boundingBox)
    {
        BoundingBox = boundingBox;
    }

    protected void HandlePlayerPowerUp(Player player)
    {
        if (player.BoundingSphere.Intersects(BoundingBox))
        {
            ApplyPowerUp(player);
        }
    }
    
    protected abstract void ApplyPowerUp(Player player);
}