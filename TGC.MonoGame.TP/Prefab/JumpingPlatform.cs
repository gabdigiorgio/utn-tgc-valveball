using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Prefab;

public class JumpingPlatform : Platform
{
    public JumpingPlatform(Vector3 scale, Vector3 position, Material.Material material, float tiling) 
        : base(scale, position, material, tiling)
    {
    }
    
    public override void Update(GameTime gameTime, Player.Player player)
    {
        if (BoundingBox.Intersects(player.BoundingSphere))
        {
            player.StartJump();
        }
    }
}