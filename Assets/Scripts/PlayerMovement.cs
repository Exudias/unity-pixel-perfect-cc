using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PPController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float jumpForce = 20;
    [SerializeField] private float gravity = 10;

    private LayerMask solidMask;
    private Vector2 velocity;

    private BoxCollider2D coll;
    private PPController controller;

    private bool grounded;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        controller = GetComponent<PPController>();
        solidMask = controller.GetSolidMask();
        grounded = false;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        velocity = new Vector2(horizontalInput * topSpeed, velocity.y);

        GroundCheck();

        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpForce;
            }
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        if (velocity.x != 0)
        {
            controller.MoveH(velocity.x * Time.deltaTime, onCollide: OnCollideH);
        }
        if (velocity.y != 0)
        {
            controller.MoveV(velocity.y * Time.deltaTime, onCollide: OnCollideV);
        }
    }

    private void GroundCheck()
    {
        grounded = CollisionHelper.CheckColliderUnitOffset(coll, Vector2.down, solidMask) != null;
    }

    private void OnCollideH(Collider2D hit)
    {
        velocity.x = 0;
    }

    private void OnCollideV(Collider2D hit)
    {
        velocity.y = 0;
    }
}
