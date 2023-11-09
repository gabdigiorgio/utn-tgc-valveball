using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Platform;

public class MovingPlatform : Platform
{
    private Vector3 _direction = Vector3.Forward;
    private const float MaxHorizontalSpeed = 1.3f;

    public MovingPlatform(Vector3 scale, Vector3 position, Material material = null)
        : base(scale, position, material)
    {
    }

    public override void Update()
    {
        SolveXCollisions();
        var increment = _direction * MaxHorizontalSpeed;
        PreviousPosition = Position;
        Position += increment;
        BoundingBox = new BoundingBox(BoundingBox.Min + increment, BoundingBox.Max + increment);
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }

    private void SolveXCollisions()
    {
        foreach (var prefab in PrefabManager.Prefabs)
        {
            if (prefab is Platform platform && prefab != this && Intersects(platform))
            {
                _direction *= -1;
            }
        }
    }
}