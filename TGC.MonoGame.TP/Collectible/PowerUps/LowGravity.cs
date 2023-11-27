using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Collectible.PowerUps;

public class LowGravity : PowerUp
{
    private const float DefaultScale = 0.5f;
    
    public LowGravity(Vector3 position)
        : base(new BoundingBox(new Vector3(-2, 0, -10) + position, new Vector3(2, 18, 10) + position))
    {
        Position = position;
        Scale = DefaultScale;
        PowerUpDuration = 5f;
        World = Matrix.CreateScale(DefaultScale) * Matrix.CreateTranslation(position);
    }

    protected override void SetPowerUp(Player.Player player)
    {
        player.Gravity = 60f;
        player.ApplyLowGravityPowerUp(60f);
    }
    protected override void ResetPowerUp(Player.Player player)
    {
        player.ResetGravity();
        player.ApplyLowGravityPowerUp(-60f);
    }
}