using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP;

public class Star
{
    public Matrix World { get; private set; }
    private Vector3 Position { get; set; }
    public BoundingBox BoundingBox { get; private set; }
    private float Scale { get; }

    private const float Amplitude = 0.15f;
    private const float MaxVerticalSpeed = 2f;

    public Star(Vector3 position, float scale)
    {
        Position = position;
        Scale = scale;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        var boundingBoxPosition = new Vector3(0f, position.Y + 10f, 0f);
        var boundingBoxScale = new Vector3(10f, 40f, 35f);
        var boundingBoxWorld = Matrix.CreateScale(boundingBoxScale) * Matrix.CreateTranslation(boundingBoxPosition) * World;
        BoundingBox = BoundingVolumesExtensions.FromMatrix(boundingBoxWorld);
    }

    public void Update(GameTime gameTime)
    {
        var time = (float)gameTime.TotalGameTime.TotalSeconds;

        var verticalOffset = Amplitude * (float)Math.Cos(time * MaxVerticalSpeed);

        Position = new Vector3(Position.X, Position.Y + verticalOffset, Position.Z);

        BoundingBox = new BoundingBox(BoundingBox.Min + Vector3.Up * verticalOffset, BoundingBox.Max + Vector3.Up * verticalOffset);

        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }

}