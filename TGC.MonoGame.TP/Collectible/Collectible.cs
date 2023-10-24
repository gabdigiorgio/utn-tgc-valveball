using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Collectible;

public abstract class Collectible
{
    public BoundingBox BoundingBox { get; private set; }
    private bool CanInteract { get; set; }
    public bool ShouldDraw { get; private set; }
    protected Vector3 Position { get; set; }
    protected float Scale { get; init; }
    public Matrix World { get; protected set; }
    public Model Model { get; set; }
    public Effect Shader { get; set; }
    
    private const float Amplitude = 0.15f;
    private const float VerticalSpeed = 2f;
    private const float RotationSpeed = 1.25f;
    
    protected Collectible(BoundingBox boundingBox)
    {
        BoundingBox = boundingBox;
        CanInteract = true;
        ShouldDraw = true;
        Model = null;
    }

    public virtual void Update(GameTime gameTime, Player player)
    {
        HandleCollection(player);
        UpdateAnimation(gameTime);
    }
    
    private void UpdateAnimation(GameTime gameTime)
    {
        var totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        var verticalOffset = Amplitude * (float)Math.Cos(totalTime * VerticalSpeed);
        var rotationAngle = totalTime * RotationSpeed;
        Position = new Vector3(Position.X, Position.Y + verticalOffset, Position.Z);
        BoundingBox = new BoundingBox(BoundingBox.Min + Vector3.Up * verticalOffset, BoundingBox.Max + Vector3.Up * verticalOffset);
        World = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(rotationAngle) * Matrix.CreateTranslation(Position);
    }

    private void HandleCollection(Player player)
    {
        if (!CanInteract || !player.BoundingSphere.Intersects(BoundingBox)) return;
        OnCollected(player);
        ShouldDraw = false;
        CanInteract = false;
    }

    protected abstract void OnCollected(Player player);
}
