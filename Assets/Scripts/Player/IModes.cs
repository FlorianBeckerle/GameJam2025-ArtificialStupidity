using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Player
{
    public interface IMovement
    {
        void Enter();
        void Exit();
        void Tick(float dt);
    }

    public interface IInteractMode
    {
        void Enter();
        void Exit();
    }

    public interface IInputRouter
    {
        Vector2 Move { get; }
        bool RunHeld { get; }
        
        bool JumpPressed { get; }
        
        void SwitchMode(MovementMode mode);
        void Tick();
    }

    public interface ICameraRig
    {
        void Enter();
        void Exit();
    }

    public enum MovementMode
    {
        Default,
        Classic,
        NONE,
    }
}