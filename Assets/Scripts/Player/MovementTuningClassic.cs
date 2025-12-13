using UnityEngine;

[CreateAssetMenu(fileName = "MovementTuningClassic", menuName = "Scriptable Objects/MovementTuningClassic")]
public class MovementTuningClassic : ScriptableObject
{

    [Header("Speeds")]
    public float walkSpeed = 6f;
    public float runSpeed  = 9f;
    public float rampUpSpeed = 3f;

    [Header("Acceleration / Deceleration")]
    public float groundAcceleration = 40f;
    public float groundDeceleration = 50f;
    public float airAcceleration    = 25f;
    public float airDeceleration    = 25f;

    [Header("Jumping")]
    public float jumpForce        = 14f;
    public float coyoteTime       = 0.10f;
    public float jumpBuffer       = 0.10f;
    public float lowJumpMultiplier = 1.4f; // for variable height when jump is released early

    [Header("Gravity / Fall")]
    public float gravity        = -35f;
    public float fallMultiplier = 1.8f;
    public float maxFallSpeed   = -25f;

    [Header("Landing & Misc")]
    public float landingInertia = 0.15f; // keep a bit of x vel on landing
    public bool  flipByScale    = true;  // true: scale.x flip, false: rotate Y

    [Header("Controller")]
    public float skinWidthPad = 0.0f; // if you added any wall-stick protection using skin width

    [Header("Debug")]
    public bool drawGroundedGizmo = false;
}
