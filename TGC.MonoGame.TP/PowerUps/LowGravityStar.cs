using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public class LowGravityStar : PowerUp
{
    public LowGravityStar(Vector3 position, float scale)
        : base(new BoundingBox(new Vector3(-2, 0, -10) + position, new Vector3(2, 18, 10) + position))
    {
        Position = position;
        Scale = scale;
        PowerUpDuration = 5f;
        IsPowerUpActive = false;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
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