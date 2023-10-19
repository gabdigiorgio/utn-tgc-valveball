using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP;

public class Material
{
    public float Acceleration { get; private set; }
    public float MaxJumpHeight { get; private set; }
    public float MaxSpeed { get; private set; }
    public Texture2D Texture { get; private set; }

    private Material(float acceleration = 60f, float maxJumpHeight = 35f, float maxSpeed = 180f)
    {
        Acceleration = acceleration;
        MaxJumpHeight = maxJumpHeight;
        MaxSpeed = maxSpeed;
    }

    public void LoadTexture(Texture2D texture)
    {
        Texture = texture;
    }

    public static readonly Material Marble = new(acceleration: 30f);
    public static readonly Material Rubber = new(maxJumpHeight: 40f);
    public static readonly Material Metal = new(acceleration: 100f, maxSpeed: 230f);
    
}