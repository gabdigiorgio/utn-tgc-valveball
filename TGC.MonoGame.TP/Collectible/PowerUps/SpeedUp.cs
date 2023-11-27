using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Collectible.PowerUps;

public class SpeedUp : PowerUp
{
    private const float SpeedIncrement = 70f;
    private const float DefaultScale = 0.5f;
    public SpeedUp(Vector3 position)
        : base(new BoundingBox(new Vector3(-2, -14, -16) + position, new Vector3(2, 20, 23) + position))
    {
        Position = position;
        Scale = DefaultScale;
        PowerUpDuration = 5f;
        World = Matrix.CreateScale(DefaultScale) * Matrix.CreateTranslation(position);
    }

    protected override void SetPowerUp(Player.Player player)
    {
        TGCGame.TargetCamera.Shake(3.5f, 0.5f);
        
        player.ApplySpeedPowerUp(SpeedIncrement, SpeedIncrement);
    }
    protected override void ResetPowerUp(Player.Player player)
    {
        player.ApplySpeedPowerUp(-SpeedIncrement, -SpeedIncrement);
    }
}