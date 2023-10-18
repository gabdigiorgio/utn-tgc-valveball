
namespace TGC.MonoGame.TP;

public class Material
{
    public float Acceleration { get; set; }
    public float MaxJumpHeight { get; set; }
    public float MaxSpeed { get; set; }
    
    public Material(float acceleration, float maxJumpHeight, float maxSpeed)
    {
        Acceleration = acceleration;
        MaxJumpHeight = maxJumpHeight;
        MaxSpeed = maxSpeed;
    }
    
    //public static Material Marble { get; } = new Material(marbleTexture, 30f, 20f, 100f);
    //public static Material Rubber { get; } = new Material(rubberTexture, 20f, 40f, 80f);
    //public static Material Metal { get; } = new Material(metalTexture, 50f, 30f, 120f);
}