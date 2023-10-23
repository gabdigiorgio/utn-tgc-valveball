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
        PowerUpDuration = 5f;
        IsPowerUpActive = false;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
    }

    public void Update(GameTime gameTime)
    {
        HandlePlayerPowerUp(TGCGame.Player, gameTime);
        var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        var verticalOffset = Amplitude * (float)Math.Cos(totalTime * MaxVerticalSpeed);
        Position = new Vector3(Position.X, Position.Y + verticalOffset, Position.Z);
        BoundingBox = new BoundingBox(BoundingBox.Min + Vector3.Up * verticalOffset, BoundingBox.Max + Vector3.Up * verticalOffset);
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }

    protected override void SetPowerUp(Player player)
    {
        player.Gravity = 60f;
    }
    protected override void ResetPowerUp(Player player)
    {
        player.ResetGravity();
    }
}