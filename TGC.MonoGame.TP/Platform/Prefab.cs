using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Platform;

public abstract class Prefab
{
    public Matrix World;
    public Material Material { get; set; }
    
    public abstract bool Intersects(BoundingSphere sphere);
    
    public abstract Vector3 ClosestPoint(Vector3 sphereCenter);
    
    public abstract float MaxY();

    protected Prefab(Vector3 scale, Vector3 position, Material material = null)
    {
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        Material = material ?? Material.Default;
    }
    
    protected Prefab(Vector3 scale, Vector3 position) : this(scale, position, Material.Default)
    {
    }
}