using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.Platform;

public class Ramp : Prefab
{
    public OrientedBoundingBox OrientedBoundingBox { get; set; }

    public Ramp(Vector3 scale, Vector3 position, float angleX, float angleZ, Material material) : base(scale, position, material)
    {
        var temporaryCubeAabb = BoundingVolumesExtensions.FromMatrix(Matrix.CreateScale(scale) * Matrix.CreateTranslation(position));
        var rampObb = OrientedBoundingBox.FromAABB(temporaryCubeAabb);
        rampObb.Rotate(Matrix.CreateRotationX(angleX) * Matrix.CreateRotationZ(angleZ));
        OrientedBoundingBox = rampObb;
        World = Matrix.CreateScale(scale) * Matrix.CreateRotationX(angleX) 
                                          * Matrix.CreateRotationZ(angleZ) * Matrix.CreateTranslation(position);
    }
    
    public override bool Intersects(BoundingSphere sphere)
    {
        return OrientedBoundingBox.Intersects(sphere, out _, out _);
    }
    
    public override Vector3 ClosestPoint(Vector3 sphereCenter)
    {
        return OrientedBoundingBox.ClosestPoint(sphereCenter);
    }
    
    public override float MaxY()
    {
        return 0;
    }
    
    public Ramp(Vector3 scale, Vector3 position, float angleX, float angleZ) : this(scale, position, angleX, angleZ, Material.Default)
    {
    }
}