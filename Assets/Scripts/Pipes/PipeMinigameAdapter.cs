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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMinigameMusic(3);

        else Debug.LogWarning("No AudioManager found in scene.", this);

        pipe.Solved += HandleSolved;
        pipe.Failed += HandleFailed;
    }

    private void HandleSolved()
    {
        Unhook();
        Solved?.Invoke();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBackgroundMusic();
    }

    private void HandleFailed()
    {
        // optional: nicht unhooken, wenn Pipe intern neu startbar ist
        Failed?.Invoke();
        AudioManager.Instance.PlayMinigameMusic(3);
    }

    private void Unhook()
    {
        if (pipe == null) return;
        pipe.Solved -= HandleSolved;
        pipe.Failed -= HandleFailed;
    }

    void OnDestroy() => Unhook();
}
