using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP.Collectible.Coins;

public class Coin : Collectible
{
    private const int Value = 1;
    private const float DefaultScale = 0.1f;

    public Coin(Vector3 position) 
        : base(new BoundingBox(new Vector3(-4, -8, -6) + position, new Vector3(4, 8, 6) + position))
    {
        Position = position;
        Scale = DefaultScale;
    }
    
    public override void Draw(GameTime gameTime, Camera camera, GraphicsDevice graphicsDevice)
    {
        TGCGame.SetEffectParameters(Shader, Material.Material.Coin, Vector2.One * 1f, World, camera);
        
        foreach (var mesh in Model.Meshes)
        {
            var meshMatrix = mesh.ParentBone.Transform;
            Shader.Parameters["World"].SetValue(meshMatrix * World);
            mesh.Draw();
        }
        DrawGizmos();
    }

    public override bool Intersects(BoundingFrustum boundingFrustum)
    {
        return BoundingBox.Intersects(boundingFrustum);
    }

    protected override void OnCollected(Player.Player player)
    {
        player.IncreaseScore(Value);
    }
}