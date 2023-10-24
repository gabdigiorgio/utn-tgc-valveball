using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Platform;

namespace TGC.MonoGame.TP;

public class MovingObstacle
{
    public Matrix World;
    public Vector3 Position;
    public Vector3 PreviousPosition;
    public BoundingBox MovingBoundingBox;
    private Vector3 _direction = Vector3.Forward;
    private readonly Vector3 _scale;

    private const float MaxHorizontalSpeed = 1f;

    private float timer = 0f;

    public MovingObstacle(Matrix world, Vector3 scale, Vector3 position, BoundingBox movingBoundingBox)
    {
        World = world;
        Position = position;
        MovingBoundingBox = movingBoundingBox;
        _scale = scale;
    }

    public void Update(GameTime gameTime)
    {
        timer += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
        var increment2 = _direction * new Vector3(0f, 0f, MathF.Sin(timer));
        PreviousPosition = Position;
        Position += increment2;
        MovingBoundingBox = new BoundingBox(MovingBoundingBox.Min + increment2, MovingBoundingBox.Max + increment2);
        World = Matrix.CreateScale(_scale) * Matrix.CreateTranslation(Position);
    }
}