using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.Prefab;

public class Platform : Prefab
{
    protected BoundingBox BoundingBox { get; set; }

    public Platform(Vector3 scale, Vector3 position, Material.Material material, float tiling) : base(scale, position, tiling, material)
    {
        BoundingBox = BoundingVolumesExtensions.FromMatrix(World);
        GizmosDrawColor = Color.Red;
    }
    
    public override bool Intersects(BoundingSphere sphere)
    {
        return BoundingBox.Intersects(sphere);
    }

    public override bool Intersects(BoundingFrustum boundingFrustum)
    {
       return boundingFrustum.Intersects(BoundingBox);
    }

    protected override Vector3 GetCenter()
    {
        return BoundingVolumesExtensions.GetCenter(BoundingBox);
    }

    protected override Vector3 GetExtents()
    {
        return BoundingVolumesExtensions.GetExtents(BoundingBox);
    }

    public override Vector3 ClosestPoint(Vector3 sphereCenter)
    {
        return BoundingVolumesExtensions.ClosestPoint(BoundingBox, sphereCenter);
    }
    
    public override float MaxY()
    {
        return BoundingBox.Max.Y;
    }
    
    public override float? Intersects(Ray ray)
    {
        return ray.Intersects(BoundingBox);
    }

    protected bool Intersects(Platform platform)
    {
        return BoundingBox.Intersects(platform.BoundingBox);
    }
    
}