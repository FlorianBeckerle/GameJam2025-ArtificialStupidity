using Player;
using UnityEngine;

public class MovementDefault : MonoBehaviour, IMovement
{

    [SerializeField] private MovementTuningDefault tuning;
    [SerializeField] MonoBehaviour inputRouter; // IInputRouter
    [SerializeField] Transform graphics;
    [SerializeField] private Rigidbody2D rb;
    
    IInputRouter input;
    
    Vector2 velocity;
    [SerializeField] private float currentSpeed;

    void ResetRotation()
    {
        graphics = transform;
    }
    
    void Awake()
    {
        input = (IInputRouter)inputRouter;
        if (!graphics) graphics = transform;
        if(!rb) rb = graphics.gameObject.GetComponent<Rigidbody2D>();
    }

    public void Enter() { enabled = true; }
    public void Exit() { enabled = false; }

    public void Tick(float dt)
    {
        //Check if enabled
        if (!enabled || tuning == null || input == null || rb == null) return;

        float baseSpeed = input.RunHeld ? tuning.runSpeed : tuning.walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, Time.deltaTime * tuning.rampUpSpeed); //Ramp up player speed
        
        Vector2 targetVel = input.Move * currentSpeed;
        
        //TODO: move to function
        bool acceleratingX = Mathf.Abs(targetVel.x) > Mathf.Abs(velocity.x);
        float accelX = acceleratingX ? tuning.groundAcceleration : tuning.groundDeceleration;
        bool acceleratingY = Mathf.Abs(targetVel.y) > Mathf.Abs(velocity.y);
        float accelY = acceleratingY ? tuning.groundAcceleration : tuning.groundDeceleration;
        //TODO: end of todo
        
        

        velocity.x = Mathf.MoveTowards(velocity.x, targetVel.x, accelX * dt);
        velocity.y = Mathf.MoveTowards(velocity.y, targetVel.y, accelY * dt);
        
        
        
        rb.MovePosition(rb.position + velocity * dt);
        
    }
}
