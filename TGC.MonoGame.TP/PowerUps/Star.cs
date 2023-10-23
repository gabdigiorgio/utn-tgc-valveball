using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public class Star : PowerUp
{
    public Matrix World { get; private set; }
    private Vector3 Position { get; set; }
    private float Scale { get; }

    private const float Amplitude = 0.15f;
    private const float MaxVerticalSpeed = 2f;

    public Star(Vector3 position, float scale)
        : base(new BoundingBox(new Vector3(-2, 0, -10) + position, new Vector3(2, 18, 10) + position))
    {
        Position = position;
        Scale = scale;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
    }

    public void Update(GameTime gameTime)
    {
        var time = (float)gameTime.TotalGameTime.TotalSeconds;
        HandlePlayerPowerUp(TGCGame.Player);
        var verticalOffset = Amplitude * (float)Math.Cos(time * MaxVerticalSpeed);
        Position = new Vector3(Position.X, Position.Y + verticalOffset, Position.Z);
        BoundingBox = new BoundingBox(BoundingBox.Min + Vector3.Up * verticalOffset, BoundingBox.Max + Vector3.Up * verticalOffset);
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }
    
    protected override void ApplyPowerUp(Player player)
    {
        player.Gravity = 60f;
    }
}