using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP.Collectible.Checkpoints;

public class FinalCheckpoint : Collectible
{
    private const float DefaultScale = 0.1f;
    private const float DefaultXRotation = -MathHelper.PiOver2;

    public FinalCheckpoint(Vector3 position) 
        : base(new BoundingBox(new Vector3(-4, -0, -4) + position, new Vector3(4, 22, 4) + position))
    {
        Position = position;
        Scale = DefaultScale;
        World = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(DefaultXRotation) * Matrix.CreateTranslation(position);
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
    
    protected override void UpdateAnimation(GameTime gameTime)
    { }
    
    protected override void StopDrawing()
    { }

    protected override void OnCollected(Player.Player player)
    {
        TGCGame.EndGame();
    }
}