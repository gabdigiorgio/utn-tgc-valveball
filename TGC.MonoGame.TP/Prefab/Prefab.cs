using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Prefab;

public abstract class Prefab
{
    public Matrix World;
    protected Vector3 Scale { get; }
    public Vector3 Position { get; set; }
    public Vector3? PreviousPosition { get; protected set; }
    public Material Material { get; }
    
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
    public virtual void Update(GameTime gameTime)
    {
    }
}