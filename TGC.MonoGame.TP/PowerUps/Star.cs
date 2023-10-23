using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.PowerUps;

public class Star : PowerUp
{
    public Matrix World { get; private set; }
    private Vector3 Position { get; set; }
    private float Scale { get; }
    private float _elapsedTimeSinceActivation;
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
        var time = (float)gameTime.TotalGameTime.TotalSeconds;
        var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        HandlePlayerPowerUp(TGCGame.Player);
        var verticalOffset = Amplitude * (float)Math.Cos(time * MaxVerticalSpeed);
        Position = new Vector3(Position.X, Position.Y + verticalOffset, Position.Z);
        BoundingBox = new BoundingBox(BoundingBox.Min + Vector3.Up * verticalOffset, BoundingBox.Max + Vector3.Up * verticalOffset);
        World = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
        
        if (IsPowerUpActive)
        {
            _elapsedTimeSinceActivation += elapsedTime;
            
            if (_elapsedTimeSinceActivation >= PowerUpDuration)
            {
                TGCGame.Player.ResetGravity();
                IsPowerUpActive = false;
                _elapsedTimeSinceActivation = 0f;
            }
        }
    }
    
    protected override void ApplyPowerUp(Player player)
    {
        player.Gravity = 60f;
        IsPowerUpActive = true;
    }
}