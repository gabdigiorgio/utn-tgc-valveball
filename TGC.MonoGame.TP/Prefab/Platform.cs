using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.Platform;

public class Platform : Prefab
{
    public BoundingBox BoundingBox { get; set; }

    public Platform(Vector3 scale, Vector3 position, Material material) : base(scale, position, material)
    {
        BoundingBox = BoundingVolumesExtensions.FromMatrix(World);
    }
    
    public override bool Intersects(BoundingSphere sphere)
    {
        return BoundingBox.Intersects(sphere);
    }
    
    public override Vector3 ClosestPoint(Vector3 sphereCenter)
    {
        return BoundingVolumesExtensions.ClosestPoint(BoundingBox, sphereCenter);
    }
    
    public override float MaxY()
    {
        return BoundingBox.Max.Y;
    }

    protected bool Intersects(Platform platform)
    {
        return BoundingBox.Intersects(platform.BoundingBox);
    }
    
}