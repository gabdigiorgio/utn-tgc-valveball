using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;

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
    
    public override void Draw(GameTime gameTime, Camera camera, GraphicsDevice graphicsDevice){
        Shader.Parameters["View"].SetValue(camera.View);
        Shader.Parameters["Projection"].SetValue(camera.Projection);
        Shader.Parameters["DiffuseColor"]?.SetValue(Color.Yellow.ToVector3());
        Shader.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

        foreach (var mesh in Model.Meshes)
        {
            var meshMatrix = mesh.ParentBone.Transform;
            Shader.Parameters["World"].SetValue(meshMatrix * World);
            mesh.Draw();
        }
        
        DrawGizmos();
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