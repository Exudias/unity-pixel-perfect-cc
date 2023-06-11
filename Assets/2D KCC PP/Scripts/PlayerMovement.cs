using UnityEngine;

[RequireComponent(typeof(PPController))]
public class PlayerMovement : MonoBehaviour
{
    #region Serialized Variables
    [Header("Horizontal Movement")]
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float acceleration = 50;
    [SerializeField] private float deceleration = 100;
    [SerializeField] private float pastMaxAccelerationMultiplier = 0.5f;
    [SerializeField] private float airControlMultiplier = 0.5f;
    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 20;
    [SerializeField] private float upwardsGravity = 80;
    [SerializeField] private float downwardsGravity = 100;
    [SerializeField] private float apexVelocityThreshold = 0;
    [SerializeField] private float apexVelocityMultiplier = 1;
    [SerializeField] private float apexAccelMultiplier = 1;
    [SerializeField] private float apexGravityMultiplier = 1;
    [SerializeField] private float maxFallSpeed = -10f;
    [SerializeField] private float pastMaxFallSpeedDeceleration = 150;
    [Header("Leniency")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    [SerializeField] private float jumpReleaseMultiplier = 0.5f;
    #endregion

    #region Private Variables
    private LayerMask solidMask;
    private Vector2 velocity;

    // Leniency variables
    private float timeSinceGrounded;
    private float timeSinceJumpInput;

    // References
    private BoxCollider2D coll;
    private PPController controller;

    // Jumping variables
    private bool grounded;
    private bool jumping;
    private bool hasReleasedJump;
    #endregion

    #region MonoBehaviour Functions
    private void Awake()
    {
        GetLocalComponents();
    }

    private void Start()
    {
        solidMask = controller.GetSolidMask();
        InitializeVariables();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Initialize values so they may be modified by certain conditions
        float currentMaxHorizontalSpeed = topSpeed;
        float currentAcceleration = acceleration;
        float currentGravity = upwardsGravity;

        bool shouldDecelerate = horizontalInput == 0 || horizontalInput * velocity.x < 0;
        if (shouldDecelerate)
        {
            currentAcceleration = deceleration;
        }

        IncrementCounters();

        if (velocity.y <= 0)
        {
            GroundCheck();
            currentGravity = downwardsGravity; // for better control feel, higher grav downwards
        }

        // get jump buffer
        if (Input.GetKeyDown(KeyCode.Space)) timeSinceJumpInput = 0;

        bool shouldCutJumpShort = jumping && !hasReleasedJump && Input.GetKeyUp(KeyCode.Space) && velocity.y > 0;
        if (shouldCutJumpShort)
        {
            velocity.y *= jumpReleaseMultiplier;
            hasReleasedJump = true;
        }

        // for more control at apex of jump
        bool shouldApplyApexMultiplier = jumping && Mathf.Abs(velocity.y) <= apexVelocityThreshold;
        if (shouldApplyApexMultiplier)
        {
            currentMaxHorizontalSpeed *= apexVelocityMultiplier;
            currentAcceleration *= apexAccelMultiplier;
            currentGravity *= apexGravityMultiplier;
        }

        if (grounded)
        {
            timeSinceGrounded = 0;
            jumping = false;
        }
        else
        {
            currentAcceleration *= airControlMultiplier; // dampen acceleration mid-air

            // clamp natural falling speed
            if (velocity.y > maxFallSpeed)
            {
                velocity.y = Mathf.Max(velocity.y - currentGravity * Time.deltaTime, maxFallSpeed);
            }
            else
            {
                velocity.y = Mathf.MoveTowards(velocity.y, maxFallSpeed, pastMaxFallSpeedDeceleration * Time.deltaTime);
            }
        }

        // jump if within coyote and jump is buffered
        if (timeSinceGrounded < coyoteTime && timeSinceJumpInput < jumpBuffer)
        {
            Jump();
        }

        // accelerate towards desired velocity, with multiplier when above top speed
        if (Mathf.Abs(velocity.x) > currentMaxHorizontalSpeed)
        {
            currentAcceleration *= pastMaxAccelerationMultiplier;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, horizontalInput * currentMaxHorizontalSpeed, currentAcceleration * Time.deltaTime);

        if (velocity.x != 0)
        {
            controller.MoveH(velocity.x * Time.deltaTime, onCollide: OnCollideH);
        }
        if (velocity.y != 0)
        {
            controller.MoveV(velocity.y * Time.deltaTime, onCollide: OnCollideV);
        }
    }
    #endregion

    #region Movement & Collision
    private void Jump()
    {
        velocity.y = jumpForce;

        jumping = true;
        grounded = false;
        hasReleasedJump = false;
        timeSinceGrounded = Mathf.Infinity;
        timeSinceJumpInput = Mathf.Infinity;
    }

    private void OnCollideH(Collider2D hit)
    {
        velocity.x = 0;
    }

    private void OnCollideV(Collider2D hit)
    {
        velocity.y = 0;
    }

    private void GroundCheck()
    {
        grounded = CollisionHelper.CheckColliderUnitOffset(coll, Vector2.down, solidMask) != null;
    }
    #endregion

    #region Utility
    private void GetLocalComponents()
    {
        coll = GetComponent<BoxCollider2D>();
        controller = GetComponent<PPController>();
    }

    private void InitializeVariables()
    {
        grounded = false;
        jumping = false;
        hasReleasedJump = false;
        timeSinceGrounded = Mathf.Infinity;
        timeSinceJumpInput = Mathf.Infinity;
    }

    private void IncrementCounters()
    {
        timeSinceGrounded += Time.deltaTime;
        timeSinceJumpInput += Time.deltaTime;
    }
    #endregion
}
