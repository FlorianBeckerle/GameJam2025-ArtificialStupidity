using UnityEngine;
using System;  

public class LevelSectionMinigameAdapter : MonoBehaviour, INpcMinigame
{
    public event Action Solved;
    public event Action Failed;

    [Header("Section Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Collider2D endTrigger;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool snapToStartOnBegin = true;

    [SerializeField] private bool returnPlayerToOriginalPosition = false;

    private GameObject player;
    private Vector3 originalPlayerPosition;

    private PlayerCore playerCore; //MovementMode -> default = topdown, classic -> JumpandRun

    public void Begin()
    {
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError($"LevelSectionMinigameAdapter: Kein GameObject mit Tag '{playerTag}' gefunden.", this);
            Failed?.Invoke();
            return;
        }

        playerCore = player.GetComponent<PlayerCore>();
        if (playerCore == null)
        {
            Debug.LogError("LevelSectionMinigameAdapter: PlayerCore-Komponente nicht gefunden.", this);
            Failed?.Invoke();
            return;
        }

        if (startPoint == null)
        {
            Debug.LogError("LevelSectionMinigameAdapter: Startpunkt ist nicht gesetzt.", this);
            Failed?.Invoke();
            return;
        }

        if (endTrigger == null)
        {
            Debug.LogError("LevelSectionMinigameAdapter: End-Trigger ist nicht gesetzt.", this);
            Failed?.Invoke();
            return;
        }

        if (!endTrigger.isTrigger)
        {
            Debug.LogWarning("LevelSectionMinigameAdapter: End-Trigger ist kein Trigger. Setze isTrigger auf true.", this);
        }

        originalPlayerPosition =player.transform.position;

        if(snapToStartOnBegin)
            TeleportPlayerTo(startPoint.position);

        playerCore.SetMode(Player.MovementMode.Classic);

        endTrigger.enabled = true;
        // Hier ggf. Setup/Start Animation/UI
    }

    private void TeleportPlayerTo(Vector3 position)
    {
        var rb2D = player.GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.position = position;
            rb2D.linearVelocity = Vector2.zero; // Stoppe jegliche Bewegung
        }
        else
        {
            player.transform.position = position;
        }
    }

    // Diese Methoden rufst du auf, wenn dein LevelSection-Minispiel fertig ist:
    public void Complete()
    {
        playerCore.SetMode(Player.MovementMode.Default);

        if(returnPlayerToOriginalPosition)
            TeleportPlayerTo(originalPlayerPosition);

        Solved?.Invoke();
    }

    public void Fail()
    {
        playerCore.SetMode(Player.MovementMode.Default);
        Failed?.Invoke();
    }
}
