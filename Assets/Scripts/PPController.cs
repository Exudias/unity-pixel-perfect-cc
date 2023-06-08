using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PPController : MonoBehaviour
{
    [SerializeField] private PPSettings ppSettings;
    [SerializeField] private LayerMask solidMask;

    private Vector2 movementCounter;
    private float pixelSize;
    private BoxCollider2D coll;

    public delegate void Collision(Collider2D collider);

    private void Awake()
    {
        Physics2D.autoSyncTransforms = true; // because we use physics for collision checking
        pixelSize = 1f / ppSettings.pixelsPerUnit;
        coll = GetComponent<BoxCollider2D>();
    }

    public void MoveH(float amount, bool collideSolids = true, Collision onCollide = null)
    {
        movementCounter.x += amount;
        float moveDir = Mathf.Sign(amount);
        while (Mathf.Abs(movementCounter.x) >= pixelSize)
        {
            if (collideSolids)
            {
                Vector3 collisionCheckOffset = Vector2.right * moveDir * pixelSize;
                Vector2 colliderBounds = (Vector2)coll.bounds.size - Vector2.one * pixelSize;
                Collider2D hit = Physics2D.OverlapBox(coll.bounds.center + collisionCheckOffset, colliderBounds, 0, solidMask);
                if (hit != null)
                {
                    movementCounter.x = 0;
                    onCollide(hit);
                    return;
                }
            }
            movementCounter.x -= moveDir * pixelSize;
            transform.Translate(Vector2.right * moveDir * pixelSize);
        }
    }

    public void MoveV(float amount, bool collideSolids = true, Collision onCollide = null)
    {
        movementCounter.y += amount;
        float moveDir = Mathf.Sign(amount);
        while (Mathf.Abs(movementCounter.y) >= pixelSize)
        {
            if (collideSolids)
            {
                Vector3 collisionCheckOffset = Vector2.up * moveDir * pixelSize;
                Vector2 colliderBounds = (Vector2)coll.bounds.size - Vector2.one * pixelSize;
                Collider2D hit = Physics2D.OverlapBox(coll.bounds.center + collisionCheckOffset, colliderBounds, 0, solidMask);
                if (hit != null)
                {
                    movementCounter.y = 0;
                    onCollide(hit);
                    return;
                }
            }
            movementCounter.y -= moveDir * pixelSize;
            transform.Translate(Vector2.up * moveDir * pixelSize);
        }
    }

    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        MoveH(input.x * 10 * Time.deltaTime, true, OnCollideH);
        MoveV(input.y * 10 * Time.deltaTime, true, OnCollideV);
    }

    private void OnCollideH(Collider2D hit)
    {
        Debug.Log("Horizontally hit " + hit.name);
    }

    private void OnCollideV(Collider2D hit)
    {
        Debug.Log("Vertically hit " + hit.name);
    }
}
