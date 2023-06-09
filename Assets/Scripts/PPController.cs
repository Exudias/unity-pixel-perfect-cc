using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PPController : MonoBehaviour
{
    [SerializeField] private LayerMask solidMask;

    private Vector2 movementCounter;
    private float pixelSize;
    private BoxCollider2D coll;

    private const int MOVE_LENIENCY = 2; // pixels you can slip by when clipping a lip

    public delegate void Collision(Collider2D collider);

    public LayerMask GetSolidMask() => solidMask;

    private void Awake()
    {
        Physics2D.autoSyncTransforms = true; // because we use physics for collision checking
        pixelSize = 1f / PPSettings.PIXELS_PER_UNIT;
        coll = GetComponent<BoxCollider2D>();
    }

    public void MoveH(float amount, bool collideSolids = true, Collision onCollide = null, bool hasLeniency = false)
    {
        movementCounter.x += amount;
        float moveDir = Mathf.Sign(amount);
        while (Mathf.Abs(movementCounter.x) >= pixelSize)
        {
            if (collideSolids)
            {
                Vector3 collisionCheckOffset = Vector2.right * moveDir * pixelSize;
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
            movementCounter.x -= moveDir * pixelSize;
            transform.Translate(Vector2.right * moveDir * pixelSize);
        }
    }

    public void MoveV(float amount, bool collideSolids = true, Collision onCollide = null, bool hasLeniencyUp = true, bool hasLeniencyDown = false)
    {
        movementCounter.y += amount;
        float moveDir = Mathf.Sign(amount);
        while (Mathf.Abs(movementCounter.y) >= pixelSize)
        {
            if (collideSolids)
            {
                Vector3 collisionCheckOffset = Vector2.up * moveDir * pixelSize;
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
            movementCounter.y -= moveDir * pixelSize;
            transform.Translate(Vector2.up * moveDir * pixelSize);
        }
    }

    private bool LeniencyCheckAndMove(Vector2 origin, Vector2 leniencyDirection)
    {
        Vector2 unit = leniencyDirection * pixelSize;
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = 1; j <= MOVE_LENIENCY; j++)
            {
                bool canMoveThere = CollisionHelper.CheckColliderAtPoint(coll, origin + unit * i * j, solidMask) == null;
                if (canMoveThere)
                {
                    // move leniently
                    transform.Translate(unit * i * j);
                    return true;
                }
            }
        }
        return false;
    }
}
