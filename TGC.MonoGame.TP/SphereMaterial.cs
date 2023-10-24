namespace TGC.MonoGame.TP;

public class SphereMaterial
{
    public float Acceleration { get; set; }
    public float MaxJumpHeight { get; set; }
    public float MaxSpeed { get; set; }
    public Material Material { get; private set; }

    private SphereMaterial(Material material, float acceleration = 60f, float maxJumpHeight = 35f, float maxSpeed = 180f)
    {
        Material = material;
        Acceleration = acceleration;
        MaxJumpHeight = maxJumpHeight;
        MaxSpeed = maxSpeed;
    }

    public static readonly SphereMaterial SphereMarble = new(Material.Marble, acceleration: 30f);
    public static readonly SphereMaterial SphereRubber = new(Material.Rubber, maxJumpHeight: 40f);
    public static readonly SphereMaterial SphereMetal = new(Material.Metal, acceleration: 100f, maxSpeed: 230f);
}