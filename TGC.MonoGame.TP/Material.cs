using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP;

public class Material
{
    public Texture2D Diffuse { get; private set; }
    public Texture2D Normal { get; private set; }
    public Vector3 AmbientColor { get; private set; } = new(1f, 1f, 1f);
    public Vector3 DiffuseColor { get; private set; } = new(1f, 1f, 1f);
    public Vector3 SpecularColor { get; private set; } = new(1f, 1f, 1f);
    
    // Between 0-1
    public float KAmbient { get; private set; }
    public float KDiffuse { get; private set; }
    public float KSpecular { get; private set; }
    
    // Between 1-64
    public float Shininess { get; private set; }

    private Material(float kAmbient, float kDiffuse, float kSpecular, float shininess)
    {
        KAmbient = kAmbient;
        KDiffuse = kDiffuse;
        KSpecular = kSpecular;
        Shininess = shininess;
    }
    
    public static readonly Material Marble = new(0.5f, 0.320f, 0.820f, 9.820f);
    public static readonly Material Rubber = new(0.7f, 0.670f, 0.240f, 3.930f);
    public static readonly Material Metal = new(0.310f, 0.830f, 1.0f, 64.0f);
    public static readonly Material Platform = new(0.4f, 0.320f, 0.340f, 12.820f);
    public static readonly Material MovingPlatform = new(0.3f, 0.620f, 0.340f, 1.820f);

    public void LoadTexture(Texture2D diffuseTexture, Texture2D normalTexture)
    {
        Diffuse = diffuseTexture;
        Normal = normalTexture;
    }
}