using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Geometries;

namespace TGC.MonoGame.TP.Platform;

public abstract class Prefab
{
    public readonly Matrix World =  new();
    public BoxPrimitive Box { get; set; }
    public Material Material { get; set; }
}