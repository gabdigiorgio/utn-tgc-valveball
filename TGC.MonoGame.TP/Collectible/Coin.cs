using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Collectible;

public class Coin : Collectible
{
    private const int Value = 1;

    public Coin(Vector3 position, float scale) 
        : base(new BoundingBox(new Vector3(-2, -14, -16) + position, new Vector3(2, 20, 23) + position)) // TODO: ajustar boundingBox
    {
        Position = position;
        Scale = scale;
    }

    protected override void OnCollected(Player player)
    {
        player.IncreaseScore(Value);
    }
}