namespace TGC.MonoGame.TP.Player;

public class SphereMaterial
{
    public float Acceleration { get; set; }
    public float MaxJumpHeight { get; set; }
    public float MaxSpeed { get; set; }
    public Material.Material Material { get; private set; }

    private SphereMaterial(Material.Material material, float acceleration = 75f, float maxJumpHeight = 35f, float maxSpeed = 180f)
    {
        Material = material;
        Acceleration = acceleration;
        MaxJumpHeight = maxJumpHeight;
        MaxSpeed = maxSpeed;
    }

    public static readonly SphereMaterial SphereMarble = new(TP.Material.Material.Marble, acceleration: 50f);
    public static readonly SphereMaterial SphereRubber = new(TP.Material.Material.Rubber, maxJumpHeight: 40f);
    public static readonly SphereMaterial SphereMetal = new(TP.Material.Material.Metal, acceleration: 100f, maxSpeed: 230f);
}