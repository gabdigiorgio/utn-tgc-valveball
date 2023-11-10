using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Prefab;

public class MovingObstacle : MovingPlatform
{
    public MovingObstacle(Vector3 scale, Vector3 position, Material material = null)
        : base(scale, position, material)
    {
    }
    
    public override void Update(GameTime gameTime)
    {
        var increment = Move(gameTime);
        UpdateBoundingBox(increment);
        UpdateWorldMatrix();
    }
    
    protected override Vector3 Move(GameTime gameTime)
    {
        var totalTime = Convert.ToSingle(gameTime.TotalGameTime.TotalSeconds);
        var increment = Direction * new Vector3(0f, 0f, MathF.Sin(totalTime));
        PreviousPosition = Position;
        Position += increment;
        return increment;
    }
}