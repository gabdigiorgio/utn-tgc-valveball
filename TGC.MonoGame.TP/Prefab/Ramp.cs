using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.Prefab;

public sealed class Ramp : Prefab
{
    private OrientedBoundingBox OrientedBoundingBox { get; }

    public Ramp(Vector3 scale, Vector3 position, float angleX, float angleZ, Material.Material material, float tiling) : base(scale, position, tiling, material)
    {
        var temporaryCubeAabb = BoundingVolumesExtensions.FromMatrix(Matrix.CreateScale(scale) * Matrix.CreateTranslation(position));
        var rampObb = OrientedBoundingBox.FromAABB(temporaryCubeAabb);
        rampObb.Rotate(Matrix.CreateRotationX(angleX) * Matrix.CreateRotationZ(angleZ));
        OrientedBoundingBox = rampObb;
        World = Matrix.CreateScale(scale) * Matrix.CreateRotationX(angleX) 
                                          * Matrix.CreateRotationZ(angleZ) * Matrix.CreateTranslation(position);
        GizmosDrawColor = Color.Red;
    }

    public override bool Intersects(BoundingSphere sphere)
    {
        return OrientedBoundingBox.Intersects(sphere, out _, out _);
    }
    
    public override bool Intersects(BoundingFrustum boundingFrustum)
    {
        return true;
        //return OrientedBoundingBox.Intersects(boundingFrustum);
    }

    protected override Vector3 GetCenter()
    {
        return OrientedBoundingBox.Center;
    }

    protected override Vector3 GetExtents()
    {
        return OrientedBoundingBox.Extents;
    }

    private Matrix GetOrientation()
    {
        return OrientedBoundingBox.Orientation;
    }
    
    public override Vector3 ClosestPoint(Vector3 sphereCenter)
    {
        return OrientedBoundingBox.ClosestPoint(sphereCenter);
    }
    
    public override void DrawGizmos()
    {
        var center = GetCenter();
        var extents = GetExtents();
        var orientation = GetOrientation();
        var orientedBoundingBoxWorld = Matrix.CreateScale(extents * 2f) * orientation * Matrix.CreateTranslation(center);
        TGCGame.Gizmos.DrawCube(orientedBoundingBoxWorld, GizmosDrawColor);
    }
    
    public override float MaxY()
    {
        return 0;
    }
}