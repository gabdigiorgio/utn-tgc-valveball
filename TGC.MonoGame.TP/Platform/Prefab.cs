using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP.Platform;

public abstract class Prefab
{
    public Matrix World;
    public Vector3 Scale { get; set; }
    public Vector3 Position { get; set; }
    public Vector3? PreviousPosition { get; protected set; } = null;
    public Material Material { get; set; }
    
    public abstract bool Intersects(BoundingSphere sphere);
    
    public abstract Vector3 ClosestPoint(Vector3 sphereCenter);
    
    public abstract float MaxY();

    protected Prefab(Vector3 scale, Vector3 position, Material material = null)
    {
        Scale = scale;
        Position = position;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        Material = material ?? Material.Default;
    }
    public virtual void Update()
    {
    }
}