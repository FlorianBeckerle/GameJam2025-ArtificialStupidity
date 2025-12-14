using UnityEngine;
using System;

public class PipeMinigameAdapter : MonoBehaviour, INpcMinigame
{
    public event Action Solved;
    public event Action Failed;

    [SerializeField] private PipeManager pipe;

    void Awake()
    {
        if (pipe == null) pipe = GetComponent<PipeManager>();
    }

    public void Begin()
    {
        if (pipe == null)
        {
            Debug.LogError("PipeManager missing!", this);
            return;
        }

        pipe.Solved += HandleSolved;
        pipe.Failed += HandleFailed;
    }

    private void HandleSolved()
    {
        Unhook();
        Solved?.Invoke();
    }

    private void HandleFailed()
    {
        // optional: nicht unhooken, wenn Pipe intern neu startbar ist
        Failed?.Invoke();
    }

    private void Unhook()
    {
        if (pipe == null) return;
        pipe.Solved -= HandleSolved;
        pipe.Failed -= HandleFailed;
    }

    void OnDestroy() => Unhook();
}
