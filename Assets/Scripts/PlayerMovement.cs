using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PPController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float topSpeed = 10;
    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 20;
    [SerializeField] private float upwardsGravity = 80;
    [SerializeField] private float downwardsGravity = 100;
    [Header("Leniency")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    [SerializeField] private float jumpReleaseMultiplier = 0.5f;

    private float timeSinceGrounded;
    private float timeSinceJumpInput;
    
    private LayerMask solidMask;
    private Vector2 velocity;

    private BoxCollider2D coll;
    private PPController controller;

    private bool grounded;
    private bool jumping;
    private bool hasReleasedJump;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        controller = GetComponent<PPController>();
        solidMask = controller.GetSolidMask();
        grounded = false;
        jumping = false;
        hasReleasedJump = false;
        timeSinceGrounded = Mathf.Infinity;
        timeSinceJumpInput = Mathf.Infinity;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        velocity = new Vector2(horizontalInput * topSpeed, velocity.y);

        timeSinceGrounded += Time.deltaTime;
        timeSinceJumpInput += Time.deltaTime;

        float gravity = upwardsGravity;

        if (velocity.y <= 0)
        {
            GroundCheck();
            gravity = downwardsGravity;
        }

        if (Input.GetKeyDown(KeyCode.Space)) timeSinceJumpInput = 0;

        if (jumping && !hasReleasedJump && Input.GetKeyUp(KeyCode.Space) && velocity.y > 0)
        {
            velocity.y *= jumpReleaseMultiplier;
            hasReleasedJump = true;
        }

        if (grounded)
        {
            timeSinceGrounded = 0;
            jumping = false;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        if (timeSinceGrounded < coyoteTime && timeSinceJumpInput < jumpBuffer)
        {
            Jump();
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

    private void Jump()
    {
        jumping = true;
        grounded = false;
        hasReleasedJump = false;
        velocity.y = jumpForce;
        timeSinceGrounded = Mathf.Infinity;
        timeSinceJumpInput = Mathf.Infinity;
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
