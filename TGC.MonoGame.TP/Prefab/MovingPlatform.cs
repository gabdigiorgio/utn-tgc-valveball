using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Prefab;

public class MovingPlatform : Platform
{
    protected Vector3 Direction = Vector3.Forward;
    private const float MaxHorizontalSpeed = 1.3f;

    public MovingPlatform(Vector3 scale, Vector3 position, Material material = null)
        : base(scale, position, material)
    {
    }

    public override void Update(GameTime gameTime)
    {
        SolveXCollisions();
        var increment = Move(gameTime);
        UpdateBoundingBox(increment);
        UpdateWorldMatrix();
    }
    
    protected virtual Vector3 Move(GameTime gameTime)
    {
        var increment = Direction * MaxHorizontalSpeed;
        PreviousPosition = Position;
        Position += increment;
        return increment;
    }

    protected void UpdateBoundingBox(Vector3 increment)
    {
        BoundingBox = new BoundingBox(BoundingBox.Min + increment, BoundingBox.Max + increment);
    }

    protected void UpdateWorldMatrix()
    {
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }
    
    private void SolveXCollisions()
    {
        foreach (var prefab in PrefabManager.Prefabs)
        {
            if (prefab is Platform platform && prefab != this && Intersects(platform))
            {
                Direction *= -1;
            }
        }
    }
}