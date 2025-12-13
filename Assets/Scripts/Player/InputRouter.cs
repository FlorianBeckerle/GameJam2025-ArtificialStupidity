using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour, IInputRouter
{

    [Header("Components")] 
    [SerializeField] private InputActionAsset asset;
    private InputActionMap activeMap;
    private InputAction moveAction, interactAction, jumpAction, sprintAction;
    [SerializeField] private Animator animator;

    [Header("Inputs")] 
    [SerializeField] public Vector2 Move { get; private set; }
    [SerializeField] public bool RunHeld { get; private set; }
    [SerializeField] public bool JumpPressed { get; private set; }
    
    void OnEnable()
    {
        activeMap?.Enable();
        SubscribeActions(); // safe if nulls
    }

    void OnDisable()
    {
        UnsubscribeActions();
        activeMap?.Disable();
    }
    
    void UnsubscribeActions()
    {
        if (interactAction != null) interactAction.performed -= OnInteractPerformed;
        if (jumpAction != null) jumpAction.performed -= OnJumpPerformed;

        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled  -= OnSprintCanceled;
        }
    }

    void SubscribeActions()
    {
        if (interactAction != null) interactAction.performed += OnInteractPerformed;
        if (jumpAction != null)
        {
            jumpAction.performed += OnJumpPerformed;
            jumpAction.canceled += OnJumpCanceled;
        }

        if (sprintAction != null)
        {
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled  += OnSprintCanceled;
        }
    }

    

    public void SwitchMode(MovementMode mode)
    {
        string target = mode.ToString(); //get string value of current mode
        
        
        if (activeMap != null && activeMap.name == target)
        {
            Debug.Log($"[InputRouter] Map already set: {target}");
            return;
        }

        activeMap?.Disable();
        
        UnsubscribeActions();
        BindMap(target);
        SubscribeActions();
        
        activeMap?.Enable();
        Debug.Log($"[InputRouter] Switched to map: {target}");
    }
    
    //Basically update but called by player instead of frames
    public void Tick()
    {
        if (moveAction == null) return;
        
        Move = moveAction.ReadValue<Vector2>();
        if (Move.x != 0 || Move.y != 0)
        {
            animator.SetBool("isWalking", true);
            animator.speed = 1;
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.speed = 0;
            animator.playbackTime = 0;
        }
        
        
        
        
    }

    //Bin current Input Map to player controller
    void BindMap(string mapName)
    {
        Debug.Log($"[InputRouter] Binding map: {mapName}");
        activeMap = asset.FindActionMap(mapName);
        
        //Check if map was found
        if (activeMap == null)
        {
            Debug.LogError($"[InputRouter] Could not find action map: {mapName}");
            return;
        }
        
        //bind Actions
        moveAction = activeMap.FindAction("Move");
        interactAction = activeMap.FindAction("Interact");
        jumpAction = activeMap.FindAction("Jump");
        sprintAction = activeMap.FindAction("Sprint");
        
        //Log if something was not found:
        if(moveAction == null) Debug.LogError($"[InputRouter] 'Move' action missing in map: {mapName}.");
        if(interactAction == null) Debug.LogError($"[InputRouter] 'Interact' action missing in map: {mapName}.");
        if(jumpAction == null) Debug.LogError($"[InputRouter] 'Jump' action missing in map: {mapName}.");
        if(sprintAction == null) Debug.LogError($"[InputRouter] 'Sprint' action missing in map: {mapName}.");
        
    }

    void OnInteractPerformed(InputAction.CallbackContext _)
    {
        //TODO: handle Interact Action
    }

    void OnJumpPerformed(InputAction.CallbackContext _)
    {
        JumpPressed = true;
    }
    
    private void OnJumpCanceled(InputAction.CallbackContext obj)
    {
        JumpPressed = false;
    }

    void OnSprintPerformed(InputAction.CallbackContext _)
    {
        RunHeld = true;
        animator.speed = 2;
        
    }
    
    void OnSprintCanceled(InputAction.CallbackContext _)
    {
        RunHeld = false;
        animator.speed = 1;
    }
}
