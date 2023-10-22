using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Collisions;

namespace TGC.MonoGame.TP;

public class Star
{
    public Matrix World { get; private set; }
    private Vector3 Position { get; set; }
    public BoundingBox BoundingBox { get; private set; }
    private float Scale { get; set; }

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
        var increment = Vector3.Up * 0.5f;
        Position += increment;
        BoundingBox = new BoundingBox(BoundingBox.Min + increment, BoundingBox.Max + increment);
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }
}