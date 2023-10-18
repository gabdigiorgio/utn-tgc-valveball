
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP;

public class Material
{
    public float Acceleration { get; set; }
    public float MaxJumpHeight { get; set; }
    public float MaxSpeed { get; set; }
    public Texture2D Texture { get; set; }

    private Material(float acceleration, float maxJumpHeight, float maxSpeed)
    {
        Acceleration = acceleration;
        MaxJumpHeight = maxJumpHeight;
        MaxSpeed = maxSpeed;
    }

    public void LoadTexture(Texture2D texture)
    {
        Texture = texture;
    }
    
    public static readonly Material Marble = new(30f, 35f, 180f);
    public static readonly Material Rubber = new(60f, 50f, 180f);
    public static readonly Material Metal = new(100f, 35f, 230f);
}