using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Material;

public class Material
{
    public Texture2D Diffuse { get; private set; }
    public Texture2D Normal { get; private set; }
    public Vector2 Tiling { get; private set; }
    public Vector3 AmbientColor { get; private set; } = new(1f, 1f, 1f);
    public Vector3 DiffuseColor { get; private set; } = new(1f, 1f, 0.880f);
    public Vector3 SpecularColor { get; private set; } = new(1f, 1f, 1f);
    
    // Between 0-1
    public float KAmbient { get; private set; }
    public float KDiffuse { get; private set; }
    public float KSpecular { get; private set; }
    
    // Between 1-64
    public float Shininess { get; private set; }

    private Material(float kAmbient, float kDiffuse, float kSpecular, float shininess, float tiling = 1f)
    {
        KAmbient = kAmbient;
        KDiffuse = kDiffuse;
        KSpecular = kSpecular;
        Shininess = shininess;
        Tiling = Vector2.One * tiling;
    }
    
    public static readonly Material Marble = new(0.5f, 0.430f, 0.880f, 32.820f);
    public static readonly Material Rubber = new(0.7f, 0.670f, 0.240f, 3.930f, 5f);
    public static readonly Material Metal = new(0.510f, 0.330f, 0.750f, 64.0f, 5f);
    public static readonly Material Default = new(0.5f, 0.5f, 0.1f, 0.5f);
    public static readonly Material Platform = new(0.4f, 0.450f, 0.340f, 12.820f);
    public static readonly Material PlatformBlue = new(0.4f, 0.550f, 0.340f, 12.820f);
    public static readonly Material MovingPlatform = new(0.380f, 0.620f, 0.340f, 1.820f);
    
    public static void LoadMaterials(ContentManager content)
    {
        const string contentFolderTextures = TGCGame.ContentFolderTextures;
        
        var defaultDiffuse = content.Load<Texture2D>(contentFolderTextures + "default_diffuse");
        var platformGreenDiffuse = content.Load<Texture2D>(contentFolderTextures + "platform_green_diffuse");
        var platformOrangeDiffuse = content.Load<Texture2D>(contentFolderTextures + "platform_orange_diffuse");
        var platformBlueDiffuse = content.Load<Texture2D>(contentFolderTextures + "platform_blue_diffuse");
        var marbleDiffuse = content.Load<Texture2D>(contentFolderTextures + "marble_black_diffuse");
        var rubberDiffuse = content.Load<Texture2D>(contentFolderTextures + "rubber_diffuse");
        var metalDiffuse = content.Load<Texture2D>(contentFolderTextures + "metal_diffuse");

        var defaultNormal = content.Load<Texture2D>(contentFolderTextures + "default_normal");
        var platformSquareNormal = content.Load<Texture2D>(contentFolderTextures + "platform_square_normal");
        var platformNormal = content.Load<Texture2D>(contentFolderTextures + "platform_normal");
        var platformBlueNormal = content.Load<Texture2D>(contentFolderTextures + "platform_blue_normal");
        var plainNormal = content.Load<Texture2D>(contentFolderTextures + "plain_normal");
        var rubberNormal = content.Load<Texture2D>(contentFolderTextures + "rubber_normal");
        var metalNormal = content.Load<Texture2D>(contentFolderTextures + "metal_normal");
            
        // Materials
        Default.LoadTexture(defaultDiffuse, defaultNormal);
        Platform.LoadTexture(platformGreenDiffuse, platformSquareNormal);
        MovingPlatform.LoadTexture(platformOrangeDiffuse, platformNormal);
        PlatformBlue.LoadTexture(platformBlueDiffuse, platformBlueNormal);
        Marble.LoadTexture(marbleDiffuse, plainNormal);
        Rubber.LoadTexture(rubberDiffuse, rubberNormal);
        Metal.LoadTexture(metalDiffuse, metalNormal);
    }

    private void LoadTexture(Texture2D diffuseTexture, Texture2D normalTexture)
    {
        Diffuse = diffuseTexture;
        Normal = normalTexture;
    }
}