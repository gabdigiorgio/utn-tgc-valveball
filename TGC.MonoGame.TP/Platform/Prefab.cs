using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Platform;

public abstract class Prefab
{
    public Matrix World;
    public Material Material { get; set; }

    protected Prefab(Vector3 scale, Vector3 position, Material material)
    {
        World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
        Material = material;
    }
    
}