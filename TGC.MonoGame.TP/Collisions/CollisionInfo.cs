using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Collisions;

public class CollisionInfo
{
    public Vector3 ClosestPoint { get; }
    public float Distance { get; }
    
    public Vector3? ColliderMovement { get; }

    public CollisionInfo(Vector3 closestPoint, float distance)
    {
        ClosestPoint = closestPoint;
        Distance = distance;
        ColliderMovement = new Vector3(0f, 0f, 0f);
    }
    
    public CollisionInfo(Vector3 closestPoint, float distance, Vector3? colliderMovement)
    {
        ClosestPoint = closestPoint;
        Distance = distance;
        ColliderMovement = colliderMovement;
    }
}