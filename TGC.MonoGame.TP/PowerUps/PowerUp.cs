using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public abstract class PowerUp
{
    public BoundingBox BoundingBox { get; private set; }
    protected float PowerUpDuration { get; init; }
    protected bool IsPowerUpActive { get; set; }
    private float ElapsedTimeSinceActivation { get; set; }
    private bool CanInteract { get; set; }
    public bool ShouldDraw { get; private set; }
    protected Vector3 Position { get; set; }
    protected float Scale { get; init; }       
    public Matrix World { get; protected set; }
    private const float Amplitude = 0.15f;
    private const float MaxVerticalSpeed = 2f;
    protected PowerUp(BoundingBox boundingBox)
    {
        BoundingBox = boundingBox;
        CanInteract = true;
        ShouldDraw = true;
        IsPowerUpActive = false;
    }
    
    public void Update(GameTime gameTime, Player player)
    {
        HandlePlayerPowerUp(player, gameTime);
        UpdateAnimation(gameTime);
    }

    private void UpdateAnimation(GameTime gameTime)
    {
        var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        var verticalOffset = Amplitude * (float)Math.Cos(totalTime * MaxVerticalSpeed);
        Position = new Vector3(Position.X, Position.Y + verticalOffset, Position.Z);
        BoundingBox = new BoundingBox(BoundingBox.Min + Vector3.Up * verticalOffset, BoundingBox.Max + Vector3.Up * verticalOffset);
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
    }

    private void HandlePlayerPowerUp(Player player, GameTime gameTime)
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