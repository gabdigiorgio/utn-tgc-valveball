using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Collectible.PowerUps;

public abstract class PowerUp : Collectible
{
    protected float PowerUpDuration { get; init; }
    private bool IsPowerUpActive { get; set; }
    private float ElapsedTimeSinceActivation { get; set; }
    
    protected PowerUp(BoundingBox boundingBox) : base(boundingBox)
    {
        IsPowerUpActive = false;
        ElapsedTimeSinceActivation = 0f;
    }

    public override void Update(GameTime gameTime, Player.Player player)
    {
        base.Update(gameTime, player);
        UpdatePowerUpState(gameTime, player);
    }

    protected abstract void SetPowerUp(Player.Player player);
    protected abstract void ResetPowerUp(Player.Player player);
    
    protected override void OnCollected(Player.Player player)
    {
        ActivatePowerUp(player);
    }

    private void ActivatePowerUp(Player.Player player)
    {
        SetPowerUp(player);
        IsPowerUpActive = true;
    }

    private void DeactivatePowerUp(Player.Player player)
    {
        ResetPowerUp(player);
        IsPowerUpActive = false;
        ElapsedTimeSinceActivation = 0f;
    }
    
    private void UpdatePowerUpState(GameTime gameTime, Player.Player player)
    {
        var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (!IsPowerUpActive) return;

        ElapsedTimeSinceActivation += elapsedTime;

        if (ElapsedTimeSinceActivation >= PowerUpDuration)
        {
            DeactivatePowerUp(player);
        }
    }
}