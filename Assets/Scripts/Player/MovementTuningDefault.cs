using UnityEngine;

[CreateAssetMenu(fileName = "MovementTuningDefault", menuName = "Scriptable Objects/MovementTuningDefault")]
public class MovementTuningDefault : ScriptableObject
{
    [Header("Speeds")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 2.5f;
    public float rampUpSpeed = 1.5f;
    
    [Header("Acceleration / Deceleration")]
    public float groundAcceleration = 40f;
    public float groundDeceleration = 50f;
    
}
