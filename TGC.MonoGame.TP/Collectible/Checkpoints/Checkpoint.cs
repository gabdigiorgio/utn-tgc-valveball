using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP.Collectible.Checkpoints;

public class Checkpoint : Collectible
{
    private const float DefaultScale = 0.1f;
    private const float DefaultXRotation = -MathHelper.PiOver2;

    public Checkpoint(Vector3 position) 
        : base(new BoundingBox(new Vector3(-8, -5, -8) + position, new Vector3(8, 10, 8) + position))
    {
        Position = position;
        //Scale = DefaultScale;
        World = Matrix.CreateTranslation(position);
    }
    
    public override void Draw(GameTime gameTime, Camera camera, GraphicsDevice graphicsDevice){
        graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
        graphicsDevice.BlendState = BlendState.AlphaBlend;
        
        Shader.Parameters["Texture"]?.SetValue(Material.Material.Metal.Diffuse); // test
        Shader.Parameters["AlphaFactor"].SetValue(0.5f);
        Shader.Parameters["Tint"].SetValue(Color.Red.ToVector3());
        Shader.Parameters["WorldViewProjection"].SetValue(World * camera.View * camera.Projection);
        TGCGame.CylinderPrimitive.Draw(Shader);
        
        DrawGizmos();
    }
    
    protected override void UpdateAnimation(GameTime gameTime)
    { }
    
    protected override void StopDrawing()
    { }

    protected override void OnCollected(Player.Player player)
    {
        player.ChangeRestartPosition(new Vector3(Position.X, Position.Y + 10f, Position.Z));
    }
}