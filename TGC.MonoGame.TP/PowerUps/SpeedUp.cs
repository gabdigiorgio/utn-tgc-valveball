using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public class SpeedUp : PowerUp
{
    private const float SpeedIncrement = 70f;
    public SpeedUp(Vector3 position, float scale)
        : base(new BoundingBox(new Vector3(-2, -7, -16) + position, new Vector3(2, 20, 23) + position))
    {
        Position = position;
        Scale = scale;
        PowerUpDuration = 5f;
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
    }

    protected override void SetPowerUp(Player player)
    {
        player.CurrentSphereMaterial.Acceleration += SpeedIncrement;
        player.CurrentSphereMaterial.MaxSpeed += SpeedIncrement;
    }
    protected override void ResetPowerUp(Player player)
    {
        player.CurrentSphereMaterial.Acceleration -= SpeedIncrement;
        player.CurrentSphereMaterial.MaxSpeed -= SpeedIncrement;
    }
}