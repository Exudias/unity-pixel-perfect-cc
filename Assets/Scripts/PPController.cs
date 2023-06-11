using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PPController : MonoBehaviour
{
    private const int MOVE_LENIENCY = 2; // pixels you can slip by when clipping a lip

    [SerializeField] private LayerMask solidMask;

    private BoxCollider2D coll;

    private Vector2 movementCounter;

    public delegate void Collision(Collider2D collider);
    public LayerMask GetSolidMask() => solidMask;

    private void Awake()
    {
        Physics2D.autoSyncTransforms = true; // because we use physics for collision checking
        coll = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Move the object horizontally, optionally colliding with solids with a callback (<paramref name="onCollide"/>) and also optional leniency for lips.
    /// </summary>
    /// <param name="amount">The amount (in units) to try to move.</param>
    /// <param name="collideSolids">Whether the object should stop when colliding with solids, defined by a <see cref="LayerMask"/>.</param>
    /// <param name="onCollide">The callback for when there is a collision.</param>
    /// <param name="hasLeniency">Whether to have leniency when colliding (move out of the way when hitting a solid by a few pixels).</param>
    public void MoveH(float amount, bool collideSolids = true, Collision onCollide = null, bool hasLeniency = false)
    {
        movementCounter.x += amount;
        float moveDir = Mathf.Sign(amount);
        while (Mathf.Abs(movementCounter.x) >= PPSettings.PIXEL_SIZE)
        {
            if (collideSolids)
            {
                Vector3 collisionCheckOffset = Vector2.right * moveDir * PPSettings.PIXEL_SIZE;
                Vector2 point = coll.bounds.center + collisionCheckOffset;
                Collider2D hit = CollisionHelper.CheckColliderUnitOffset(coll, Vector2.right * moveDir, solidMask);
                if (hit != null)
                {
                    if (!hasLeniency || !LeniencyCheckAndMove(point, Vector2.up))
                    {
                        movementCounter.x = 0;
                        onCollide(hit);
                        return;
                    }
                }
            }
            movementCounter.x -= moveDir * PPSettings.PIXEL_SIZE;
            transform.Translate(Vector2.right * moveDir * PPSettings.PIXEL_SIZE);
        }
    }

    /// <summary>
    /// Move the object vertically, optionally colliding with solids with a callback (<paramref name="onCollide"/>) 
    /// and also optional leniency for lips, configurable for both upwards and downwards movement.
    /// </summary>
    /// <param name="amount">The amount (in units) to try to move.</param>
    /// <param name="collideSolids">Whether the object should stop when colliding with solids, defined by a <see cref="LayerMask"/>.</param>
    /// <param name="onCollide">The callback for when there is a collision.</param>
    /// <param name="hasLeniencyUp">Whether to have leniency when colliding (move out of the way when hitting a solid by a few pixels).
    /// Only for upward movement.</param>
    /// <param name="hasLeniencyDown">Whether to have leniency when colliding (move out of the way when hitting a solid by a few pixels).
    /// Only for downward movement.</param>
    public void MoveV(float amount, bool collideSolids = true, Collision onCollide = null, bool hasLeniencyUp = true, bool hasLeniencyDown = false)
    {
        movementCounter.y += amount;
        float moveDir = Mathf.Sign(amount);
        while (Mathf.Abs(movementCounter.y) >= PPSettings.PIXEL_SIZE)
        {
            if (collideSolids)
            {
                Vector3 collisionCheckOffset = Vector2.up * moveDir * PPSettings.PIXEL_SIZE;
                Vector2 point = coll.bounds.center + collisionCheckOffset;
                Collider2D hit = CollisionHelper.CheckColliderUnitOffset(coll, Vector2.up * moveDir, solidMask);
                if (hit != null)
                {
                    bool shouldTryLenient = hasLeniencyUp && amount > 0 || hasLeniencyDown && amount < 0;
                    if (!shouldTryLenient || !LeniencyCheckAndMove(point, Vector2.right))
                    {
                        movementCounter.y = 0;
                        onCollide(hit);
                        return;
                    }
                }
            }
            movementCounter.y -= moveDir * PPSettings.PIXEL_SIZE;
            transform.Translate(Vector2.up * moveDir * PPSettings.PIXEL_SIZE);
        }
    }

    private bool LeniencyCheckAndMove(Vector2 origin, Vector2 leniencyDirection)
    {
        Vector2 unit = leniencyDirection * PPSettings.PIXEL_SIZE;
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = 1; j <= MOVE_LENIENCY; j++)
            {
                bool canMoveThere = CollisionHelper.CheckColliderAtPoint(coll, origin + unit * i * j, solidMask) == null;
                if (canMoveThere)
                {
                    transform.Translate(unit * i * j);
                    return true;
                }
            }
        }
        return false;
    }
}
