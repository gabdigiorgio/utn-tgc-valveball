using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public abstract class PowerUp
{
    public BoundingBox BoundingBox { get; protected set; }
    protected float PowerUpDuration { get; init; }
    protected bool IsPowerUpActive { get; set; }
    private float ElapsedTimeSinceActivation { get; set; }
    protected PowerUp(BoundingBox boundingBox)
    {
        BoundingBox = boundingBox;
    }

    protected void HandlePlayerPowerUp(Player player, GameTime gameTime)
    {
        var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (player.BoundingSphere.Intersects(BoundingBox))
        {
            ActivatePowerUp(player);
        }

        if (!IsPowerUpActive) return;
        ElapsedTimeSinceActivation += elapsedTime;
            
        if (ElapsedTimeSinceActivation >= PowerUpDuration)
        {
            DeactivatePowerUp(player);
        }
    }

    protected abstract void SetPowerUp(Player player);
    protected abstract void ResetPowerUp(Player player);

    private void ActivatePowerUp(Player player)
    {
        SetPowerUp(player);
        IsPowerUpActive = true;
    }

    private void DeactivatePowerUp(Player player)
    {
        ResetPowerUp(player);
        IsPowerUpActive = false;
        ElapsedTimeSinceActivation = 0f;
    }
    
}