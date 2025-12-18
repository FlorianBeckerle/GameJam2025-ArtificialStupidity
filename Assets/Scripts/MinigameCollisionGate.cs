using System.Collections.Generic;
using UnityEngine;

public class MinigameCollisionGate : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;

    private readonly List<Collider> disabled3D = new();
    private readonly List<Collider2D> disabled2D = new();
    private Transform minigameRoot;

    public void Enter(Transform minigameRootTransform)
    {
        minigameRoot = minigameRootTransform;

        disabled3D.Clear();
        disabled2D.Clear();

        foreach (var c in FindObjectsOfType<Collider>(true))
        {
            if (!c || !c.enabled) continue;
            if (IsAllowed(c.transform)) continue;

            c.enabled = false;
            disabled3D.Add(c);
        }

        foreach (var c2 in FindObjectsOfType<Collider2D>(true))
        {
            if (!c2 || !c2.enabled) continue;
            if (IsAllowed(c2.transform)) continue;

            c2.enabled = false;
            disabled2D.Add(c2);
        }
    }

    public void Exit()
    {
        foreach (var c in disabled3D) if (c) c.enabled = true;
        foreach (var c2 in disabled2D) if (c2) c2.enabled = true;

        disabled3D.Clear();
        disabled2D.Clear();
        minigameRoot = null;
    }

    private bool IsAllowed(Transform t)
    {
        if (minigameRoot && t.IsChildOf(minigameRoot)) return true;
        if (playerRoot && (t == playerRoot || t.IsChildOf(playerRoot))) return true;
        return false;
    }
}
