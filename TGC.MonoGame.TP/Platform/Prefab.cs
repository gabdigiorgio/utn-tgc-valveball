using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Geometries;

namespace TGC.MonoGame.TP.Platform;

public abstract class Prefab
{
    public readonly Matrix World;
    public BoxPrimitive Box { get; set; }
    public Material Material { get; set; }

    protected Prefab(Vector3 scale, Vector3 position)
    {
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
    }

    /*public void LoadPrefab(GraphicsDevice graphicsDevice)
    {
        Box = new BoxPrimitive(graphicsDevice, Vector3.One, Material.Diffuse);
    }*/
}