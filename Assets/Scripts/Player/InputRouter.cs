using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour, IInputRouter
{

    [Header("Components")] 
    [SerializeField] private InputActionAsset asset;
    private InputActionMap activeMap;
    private InputAction moveAction, interactAction, jumpAction, sprintAction;

    [Header("Inputs")] 
    [SerializeField] public Vector2 Move { get; private set; }
    [SerializeField] public bool RunHeld { get; private set; }

    void OnEnable()
    {
        activeMap?.Enable();
        
        if(interactAction != null)
            interactAction.performed += OnInteractPerformed;
        
        if(jumpAction != null)
            jumpAction.performed += OnJumpPerformed;

        if (sprintAction != null)
        {
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled += OnSprintCanceled;
        }
            
    }
    
    void OnDisable()
    {
        if(interactAction != null)
            interactAction.performed -= OnInteractPerformed;
        
        if(jumpAction != null)
            jumpAction.performed -= OnJumpPerformed;

        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;
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
        BindMap(target);
        activeMap?.Enable();
        Debug.Log($"[InputRouter] Switched to map: {target}");
    }
    
    //Basically update but called by player instead of frames
    public void Tick()
    {
        if (moveAction == null) return;
        
        Move = moveAction.ReadValue<Vector2>();
        
        
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
        //Not sure if needed
    }

    void OnSprintPerformed(InputAction.CallbackContext _)
    {
        RunHeld = true;
    }
    
    void OnSprintCanceled(InputAction.CallbackContext _)
    {
        RunHeld = false;
    }
}
