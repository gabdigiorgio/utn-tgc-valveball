using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public abstract class PowerUp
{
    public BoundingBox BoundingBox { get; protected set; }
    protected float PowerUpDuration { get; init; }
    protected bool IsPowerUpActive { get; set; }
    private float ElapsedTimeSinceActivation { get; set; }
    private bool CanInteract { get; set; }
    public bool ShouldDraw { get; private set; }
    protected PowerUp(BoundingBox boundingBox)
    {
        BoundingBox = boundingBox;
        CanInteract = true;
        ShouldDraw = true;
    }

    protected void HandlePlayerPowerUp(Player player, GameTime gameTime)
    {
        var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (CanInteract && player.BoundingSphere.Intersects(BoundingBox))
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
        ShouldDraw = false;
        CanInteract = false;
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