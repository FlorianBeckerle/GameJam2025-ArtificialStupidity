using Player;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    [Header("Plug-ins (assign concrete components like movement, etc.)")]
    // ----- References -----
    //Movement
    [SerializeField] private MonoBehaviour movementDefault;
    [SerializeField] private MonoBehaviour movementClassic2D;
    //Camera
    [SerializeField] private MonoBehaviour cameraBehaviour;
    //Scripts
    [SerializeField] private MonoBehaviour inputRouter;
    
    //Actual usable variables
    IMovement movement;
    IInputRouter input;
    private ICameraRig cam;
    
    MovementMode mode;

    void Awake()
    {
        //Validate
        if(!movementDefault) Debug.LogWarning("[PlayerCore] movementDefault not assigned.");
        if(!movementClassic2D) Debug.LogWarning("[PlayerCore] movementClassic2D not assigned.");
        if(!cameraBehaviour) Debug.LogWarning("[PlayerCore] cameraBehaviour not assigned.");
        if(!inputRouter) Debug.LogWarning("[PlayerCore] Input router not assigned.");
        
        input = inputRouter as IInputRouter;
        if(input == null) Debug.LogWarning("[PlayerCore] Input router not assigned or does not implement IInputRouter.");
        
        SetMode(MovementMode.Default);
    }

    void Update()
    {
        //input doesn't need to know what time has passed
        input?.Tick();
        //Movement needs delta time
        //movement?.Tick(Time.deltaTime);
    }
    
    void FixedUpdate()
    {
        movement?.Tick(Time.fixedDeltaTime);
    }

    //Use this to test Switching between modes
    void LateUpdate()
    {
        //TODO: Cycle through SETMODE, mode and output log
        // use only for debuging
    }
    
    void OnEnable(){ Debug.Log("[PlayerCore] OnEnable"); }
    void OnDisable(){ Debug.Log("[PlayerCore] OnDisable"); }
    
    //SwitchBetween different modes
    public void SetMode(MovementMode mode)
    {
        //Update mode
        this.mode = mode;
        
        //Exit Old Scripts
        movement?.Exit();
        
        //Reassign new Scripts
        switchMovementScript(mode);
        
        //Set input
        input?.SwitchMode(mode);

        //Enter new Scripts
        movement?.Enter();
        

    }

    private void switchMovementScript(MovementMode mode)
    {
        switch (mode)
        {
            case MovementMode.Default:
                movement = (IMovement)movementDefault;
                break;
            case MovementMode.Classic:
                movement = (IMovement)movementClassic2D;
                break;
            //Default Back to Default Movement mode
            case MovementMode.NONE:
            default:
                movement = (IMovement)movementDefault;
                break;
        }
    }
    
    
    void OnGUI()
    {
        GUI.Label(new Rect(10,10,600,20), $"Mode: {(mode.ToString())}");
        if (input != null)
            GUI.Label(new Rect(10,30,600,20), $"Move:{input.Move}  JumpHeld: not needed RunHeld:{input.RunHeld}");
        GUI.Label(new Rect(10,50,600,20), $"Movement assigned:{(movement!=null)}   Cam assigned:{(cam!=null)}");
    }
}
