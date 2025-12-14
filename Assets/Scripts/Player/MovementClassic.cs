using Player;
using UnityEngine;

public class MovementClassic : MonoBehaviour, IMovement
{
    
        [SerializeField] MovementTuningClassic tuning;
        [SerializeField] MonoBehaviour inputRouterMb; // IInputRouter
        [SerializeField] Transform graphics;          // flipped/rotated for facing
        [SerializeField] Rigidbody2D rb;
        
        IInputRouter input;

        // state
        Vector2 velocity;       // x used horizontally, y vertical; z stays 0
        float coyoteTimer;
        float jumpBufferTimer;
        float landingTimer;
        bool wasGrounded;
        [SerializeField] private float currentSpeed;
        
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer;

        void Reset() { graphics = transform; }

        void Awake()
        {
            input = (IInputRouter)inputRouterMb;
            rb = GetComponent<Rigidbody2D>();
            if (!graphics) graphics = transform;
        }

        public void Enter()
        {
            enabled = true;
            coyoteTimer = jumpBufferTimer = landingTimer = 0f;
            velocity = Vector3.zero;
            currentSpeed = 0;
        }

        public void Exit() { enabled = false; }

        public void Tick(float dt)
        {
            if (!enabled || tuning == null || input == null) return;

            // Grounded using CharacterController
            bool grounded = IsGrounded();

            // Choose run/walk speed
            float baseSpeed = input.RunHeld ? tuning.runSpeed : tuning.walkSpeed;

            // Horizontal target speed from input.x
            //float targetX = input.Move.x * baseSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, dt * tuning.rampUpSpeed);
            //float targetX = input.Move.x * currentSpeed;
            float targetX = input.Move.x == 0 ? 0 : Mathf.Sign(input.Move.x);
            targetX *= currentSpeed;

            // Accel/Decel depending on phase (ground/air)
            bool accelerating = Mathf.Abs(targetX) > Mathf.Abs(velocity.x);
            float accel = grounded
                ? (accelerating ? tuning.groundAcceleration : tuning.groundDeceleration)
                : (accelerating ? tuning.airAcceleration    : tuning.airDeceleration);

            velocity.x = Mathf.MoveTowards(velocity.x, targetX, accel * dt);

            // Coyote & buffer
            coyoteTimer     = grounded ? tuning.coyoteTime : Mathf.Max(0f, coyoteTimer - dt);
            if (input.JumpPressed)
            {
                jumpBufferTimer = tuning.jumpBuffer;
            }else
            {
                jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - dt);
            }

            // Jump
            if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            {
                velocity.y = tuning.jumpForce;
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }

            // Gravity with variable-height behavior
            if (velocity.y < 0f)
                velocity.y += tuning.gravity * tuning.fallMultiplier * dt;
            else if (!input.JumpPressed)
                velocity.y += tuning.gravity * tuning.lowJumpMultiplier * dt;
            else if (!input.JumpPressed)
                velocity.y += tuning.gravity * tuning.lowJumpMultiplier * dt;
            else
                velocity.y += tuning.gravity * dt;

            if (velocity.y < tuning.maxFallSpeed) velocity.y = tuning.maxFallSpeed;

            // Move strictly in X/Y (Z=0)
            Vector2 motion = new Vector3(velocity.x, velocity.y) * dt;
            //TODO: move rigidbody2D
            rb.MovePosition(rb.position + velocity * dt);

            // Landing inertia toggle
            if (!wasGrounded && grounded)
            {
                landingTimer = tuning.landingInertia;
            }
            wasGrounded = grounded;
            if (landingTimer > 0f)
            {
                landingTimer -= dt;
            }

            // Facing
            if (Mathf.Abs(input.Move.x) > 0.01f)
            {
                if (tuning.flipByScale)
                {
                    Vector3 s = graphics.localScale;
                    float sign = input.Move.x > 0 ? 1f : -1f;
                    s.x = Mathf.Abs(s.x) * sign;
                    graphics.localScale = s;
                }
                else
                {
                    Vector3 e = graphics.eulerAngles;
                    e.y = input.Move.x > 0 ? 0f : 180f;
                    graphics.eulerAngles = e;
                }
            }

            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0);
        }
        
        bool IsGrounded()
        {
            return Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

#if UNITY_EDITOR
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif

#endif
    }

