using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Collectible.Coins;

public class Coin : Collectible
{
    private const int Value = 1;
    private const float DefaultScale = 0.1f;

    public Coin(Vector3 position) 
        : base(new BoundingBox(new Vector3(-4, -8, -6) + position, new Vector3(4, 8, 6) + position))
    {
        Position = position;
        Scale = DefaultScale;
    }

    protected override void OnCollected(Player player)
    {
        player.IncreaseScore(Value);
    }
}