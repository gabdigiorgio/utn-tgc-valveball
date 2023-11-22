using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Prefab;

public abstract class Prefab
{
    protected Vector3 Scale { get; }
    public Matrix World;
    public Vector3 Position { get; protected set; }
    public Vector3? PreviousPosition { get; protected set; }
    public Material.Material Material { get; protected set; }
    public Vector2 Tiling { get; }
    protected Color GizmosDrawColor { get; init; }
    
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

    protected abstract Vector3 GetCenter();

    protected abstract Vector3 GetExtents();

    public virtual void Update(GameTime gameTime, Player.Player player)
    {
    }

    public virtual float? Intersects(Ray ray)
    {
        return null;
    }
    
    public virtual void DrawGizmos()
    {
        var center = GetCenter();
        var extents = GetExtents();
        TGCGame.Gizmos.DrawCube(center, extents * 2f, GizmosDrawColor);
    }
}