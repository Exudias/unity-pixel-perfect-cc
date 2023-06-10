using UnityEngine;

/// <summary>
/// Class for general collision-related functions.
/// </summary>
public class CollisionHelper : MonoBehaviour
{
    /// <summary>
    /// The pixel size, in Unity units, of the game.
    /// </summary>
    private static float pixelSize = 1f / PPSettings.PIXELS_PER_UNIT;

    /// <summary>
    /// Checks for a collider at <paramref name="point"/> with bounds of <paramref name="coll"/>, 
    /// filtered by <paramref name="solidMask"/>.
    /// </summary>
    /// <param name="coll">The collider to check for collision with.</param>
    /// <param name="point">The center of the collider to check with.</param>
    /// <param name="solidMask">The mask which to apply to the check.</param>
    /// <returns>The <see cref="Collider2D"/> if any was found, otherwise <see cref="null"/>.</returns>
    public static Collider2D CheckColliderAtPoint(BoxCollider2D coll, Vector2 point, LayerMask solidMask)
    {
        Vector2 colliderBounds = (Vector2)coll.bounds.size - Vector2.one * pixelSize;
        return Physics2D.OverlapBox(point, colliderBounds, 0, solidMask);
    }

    /// <summary>
    /// Checks for a collider <paramref name="offset"/> pixel units away from <paramref name="coll"/>, 
    /// filtered by <paramref name="solidMask"/>.
    /// </summary>
    /// <param name="coll">The collider to check for collision with.</param>
    /// <param name="offset">The offset from the collider, where 1 unit is 1 <see cref="pixelSize"/>.</param>
    /// <param name="solidMask">The mask which to apply to the check.</param>
    /// <returns>The <see cref="Collider2D"/> if any was found, otherwise <see cref="null"/>.</returns>
    public static Collider2D CheckColliderUnitOffset(BoxCollider2D coll, Vector2 offset, LayerMask solidMask)
    {
        Vector2 point = (Vector2)coll.bounds.center + offset * pixelSize;
        Vector2 colliderBounds = (Vector2)coll.bounds.size - Vector2.one * pixelSize;
        return Physics2D.OverlapBox(point, colliderBounds, 0, solidMask);
    }
}
