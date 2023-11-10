using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Prefab;

public abstract class Prefab
{
    protected Vector3 Scale { get; }
    public Matrix World;
    public Vector3 Position { get; protected set; }
    public Vector3? PreviousPosition { get; protected set; }
    public Material.Material Material { get; }
    public Vector2 Tiling { get; }
    
    protected Prefab(Vector3 scale, Vector3 position, float tiling, Material.Material material = null)
    {
        Scale = scale;
        Position = position;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        Material = material ?? TP.Material.Material.Default;
        Tiling = Vector2.One * tiling;
    }
    
    public abstract bool Intersects(BoundingSphere sphere);
    
    public abstract Vector3 ClosestPoint(Vector3 sphereCenter);
    
    public abstract float MaxY();
    
    public abstract Vector3 GetCenter();
    
    public abstract Vector3 GetExtents();
    
    public abstract Matrix GetOrientation();
    
    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual float? Intersects(Ray ray)
    {
        return null;
    }
}