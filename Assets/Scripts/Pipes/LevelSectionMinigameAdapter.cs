using UnityEngine;
using System;

public class LevelSectionMinigameAdapter : MonoBehaviour, INpcMinigame
{
    public event Action Solved;
    public event Action Failed;

    public bool IsRunning => started;

    [Header("Section Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Collider2D endTrigger;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool snapToStartOnBegin = true;
    [SerializeField] private bool returnPlayerToOriginalPosition = true; // <- empfehle true

    [Header("Visual Fix")]
    [SerializeField] private bool resetFacingOnBegin = true;
    [SerializeField] private bool faceRightIn2DLevel = true;
    [SerializeField] private LevelSectionChaser2D chaser;

    [SerializeField] private float triggerArmDelay = 0.2f;
    private float armAtTime;

    public bool TriggersArmed => started && Time.time >= armAtTime;
    

    private GameObject playerRoot;
    private Vector3 originalPlayerPosition;

    private PlayerCore playerCore;
    private Rigidbody2D playerRb;

    private bool started;

    public void Begin()
    {
        if(chaser == null)
            chaser = GetComponentInChildren<LevelSectionChaser2D>();
        if (started) return;
        started = true;

        var tagged = GameObject.FindGameObjectWithTag(playerTag);
        if (tagged == null)
        {
            Debug.LogError($"LevelSectionMinigameAdapter: Kein GameObject mit Tag '{playerTag}' gefunden.", this);
            started = false;
            Failed?.Invoke();
            return;
        }

        // Robust: PlayerCore kann auf Parent/Child sitzen
        playerCore = tagged.GetComponentInParent<PlayerCore>();
        if (playerCore == null) playerCore = tagged.GetComponentInChildren<PlayerCore>();

        if (playerCore == null)
        {
            Debug.LogError("LevelSectionMinigameAdapter: PlayerCore-Komponente nicht gefunden (Parent/Child geprüft).", this);
            started = false;
            Failed?.Invoke();
            return;
        }

        // Wir arbeiten ab hier immer mit dem Root, wo PlayerCore sitzt
        playerRoot = playerCore.gameObject;

        // Rigidbody2D möglichst am Root oder Parent suchen
        playerRb = playerRoot.GetComponent<Rigidbody2D>();
        if (playerRb == null) playerRb = playerRoot.GetComponentInParent<Rigidbody2D>();

        if (startPoint == null)
        {
            Debug.LogError("LevelSectionMinigameAdapter: Startpunkt ist nicht gesetzt.", this);
            started = false;
            Failed?.Invoke();
            return;
        }

        if (endTrigger == null)
        {
            Debug.LogError("LevelSectionMinigameAdapter: End-Trigger ist nicht gesetzt.", this);
            started = false;
            Failed?.Invoke();
            return;
        }

        if (!endTrigger.isTrigger)
            Debug.LogWarning("LevelSectionMinigameAdapter: End-Trigger ist kein Trigger. Setze isTrigger auf true.", this);

        originalPlayerPosition = playerRoot.transform.position;

        if (snapToStartOnBegin)
            TeleportPlayerTo(startPoint.position);
        armAtTime = Time.time + triggerArmDelay;

        if (resetFacingOnBegin)
            FixPlayerFacing();


        playerCore.SetMode(Player.MovementMode.Classic);

        endTrigger.enabled = true;
    }

    private void TeleportPlayerTo(Vector3 position)
    {
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.position = position;
            Physics2D.SyncTransforms();
        }
        else if (playerRoot != null)
        {
            playerRoot.transform.position = position;
            Physics2D.SyncTransforms();
        }
    }

    private void FixPlayerFacing()
    {
        var sprite = playerRoot.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.flipX = !faceRightIn2DLevel;
        }

        Vector3 scale = playerRoot.transform.localScale;
        scale.x = Math.Abs(scale.x) * (faceRightIn2DLevel ? 1f : -1f);
        playerRoot.transform.localScale = scale;
    }

    public void Complete()
    {
        if (!started) return;
        started = false;

        playerCore.SetMode(Player.MovementMode.Default);

        if (returnPlayerToOriginalPosition)
            TeleportPlayerTo(originalPlayerPosition);
        if(chaser != null)
            chaser.ResetChaser();

        Solved?.Invoke();
    }

    public void Fail()
    {
        if (!started) return;
        started = false;

        playerCore.SetMode(Player.MovementMode.Default);

        if (returnPlayerToOriginalPosition)
            TeleportPlayerTo(originalPlayerPosition);

        if(chaser != null)
            chaser.ResetChaser();

        Failed?.Invoke();
    }
}
