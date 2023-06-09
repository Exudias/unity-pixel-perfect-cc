using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHelper : MonoBehaviour
{
    private static float pixelSize = 1f / PPSettings.PIXELS_PER_UNIT;

    public static Collider2D CheckColliderAtPoint(BoxCollider2D coll, Vector2 point, LayerMask solidMask)
    {
        Vector2 colliderBounds = (Vector2)coll.bounds.size - Vector2.one * pixelSize;
        return Physics2D.OverlapBox(point, colliderBounds, 0, solidMask);
    }

    public static Collider2D CheckColliderUnitOffset(BoxCollider2D coll, Vector2 offset, LayerMask solidMask)
    {
        Vector2 point = (Vector2)coll.bounds.center + offset * pixelSize;
        Vector2 colliderBounds = (Vector2)coll.bounds.size - Vector2.one * pixelSize;
        return Physics2D.OverlapBox(point, colliderBounds, 0, solidMask);
    }
}
