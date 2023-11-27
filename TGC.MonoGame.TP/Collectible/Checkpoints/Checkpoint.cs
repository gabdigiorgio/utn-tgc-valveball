using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP.Collectible.Checkpoints;

public class Checkpoint : Collectible
{
    public float YawRestartPosition { get; }
    public Checkpoint(Vector3 position, float yawRestartPosition) 
        : base(new BoundingBox(new Vector3(-8, -5, -8) + position, new Vector3(8, 10, 8) + position))
    {
        Position = position;
        YawRestartPosition = yawRestartPosition;
        World = Matrix.CreateTranslation(position);
    }
    
    public override void Draw(GameTime gameTime, Camera camera, GraphicsDevice graphicsDevice){
        graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        graphicsDevice.BlendState = BlendState.AlphaBlend;

        var worldMidCylinder = Matrix.CreateScale(0.98f) * Matrix.CreateTranslation(new Vector3(Position.X, Position.Y - 1.5f, Position.Z));
        var worldShortCylinder = Matrix.CreateScale(0.94f) * Matrix.CreateTranslation(new Vector3(Position.X, Position.Y - 2.5f, Position.Z));
        
        Shader.Parameters["Texture"]?.SetValue(Material.Material.Metal.Diffuse);
        Shader.Parameters["AlphaFactor"].SetValue(0.1f);
        Shader.Parameters["Tint"].SetValue(Color.Red.ToVector3());
        Shader.Parameters["WorldViewProjection"].SetValue(World * camera.View * camera.Projection);
        TGCGame.CylinderPrimitive.Draw(Shader);
        
        Shader.Parameters["Texture"]?.SetValue(Material.Material.Metal.Diffuse);
        Shader.Parameters["AlphaFactor"].SetValue(0.15f);
        Shader.Parameters["Tint"].SetValue(Color.Red.ToVector3());
        Shader.Parameters["WorldViewProjection"].SetValue(worldMidCylinder * camera.View * camera.Projection);
        TGCGame.CylinderPrimitive.Draw(Shader);
        
        Shader.Parameters["Texture"]?.SetValue(Material.Material.Metal.Diffuse);
        Shader.Parameters["AlphaFactor"].SetValue(0.3f);
        Shader.Parameters["Tint"].SetValue(Color.Red.ToVector3());
        Shader.Parameters["WorldViewProjection"].SetValue(worldShortCylinder * camera.View * camera.Projection);
        TGCGame.CylinderPrimitive.Draw(Shader);
      
        DrawGizmos();
    }
    
    protected override void UpdateAnimation(GameTime gameTime)
    { }
    
    protected override void StopDrawing()
    { }

    protected override void OnCollected(Player.Player player)
    {
        player.ChangeRestartPosition(new Vector3(Position.X, Position.Y + 10f, Position.Z), YawRestartPosition);
    }
}