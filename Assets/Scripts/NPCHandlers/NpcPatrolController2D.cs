using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class NpcPatrolController2D : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private int startId = 0;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reachDistance = 0.1f;

    private Dictionary<int, PatrolPoint> points;
    private readonly Dictionary<int, int> prevMap = new();
    private PatrolPoint currentTarget;

    private Rigidbody2D rb;
    private bool active = true;

    private bool waitingForPlayer;
    private float ignoreObstacleUntil;

    private bool returningToPickup;
    private bool returnAfterFix;

    private bool hasPaket;



    // Zustände aus anderen Komponenten
    public bool PlayerInZone { get; internal set; }
    public bool Shortcircuited { get; internal set; }
    public bool Slipped { get; internal set; }

    public bool WaitingForPlayer => waitingForPlayer;
    public bool Active => active;

    public bool HasPaket => hasPaket;
    public bool IsMoving => active && !waitingForPlayer && !Shortcircuited && !Slipped;

    public Vector2 CurrentTargetPos => currentTarget != null ? (Vector2)currentTarget.transform.position : rb.position;
    public Vector2 MoveDirection
    {
        get
        {
            if (currentTarget == null) return Vector2.zero;
            Vector2 dir = (Vector2)currentTarget.transform.position - rb.position;
            return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.zero;
        }
    }

    public float IgnoreObstacleUntil => ignoreObstacleUntil;
    public Rigidbody2D RB => rb;

    public bool isAfterDropOffPoint => currentTarget != null && currentTarget.isDropOffPoint;
    public bool isAtPickupPoint => currentTarget != null && currentTarget.isPickupPoint;

    public event Action OnPickupArrived;
    public event Action<bool> OnWaitStateChanged; // true = waiting
    public event Action OnNeedPlayerInteract;     // optional (UI Prompt)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        CollectPatrolPoints();
        InitPrevMap();

        if (!points.TryGetValue(startId, out currentTarget))
        {
            Debug.LogError($"Starting Patrolpoint ID {startId} not found!", this);
            active = false;
            return;
        }

        // Startpos setzen, dann direkt weiter
        rb.position = currentTarget.transform.position;
        HandlePointArrival(currentTarget);
        AdvanceToNext();
    }

    void Update()
    {
        if (!active || currentTarget == null) return;

        // Kurzschluss/Oil => Controller bleibt im Waiting
        if (Shortcircuited || Slipped)
        {
            SetWaiting(true);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (waitingForPlayer)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Bewegung
        Vector2 targetPos = currentTarget.transform.position;
        Vector2 next = Vector2.MoveTowards(rb.position, targetPos, speed * Time.deltaTime);
        rb.MovePosition(next);

        if (Vector2.Distance(rb.position, targetPos) <= reachDistance)
        {
            HandlePointArrival(currentTarget);
            AdvanceToNext();
        }
    }

    public void SetWaiting(bool value)
    {
        if (waitingForPlayer == value) return;
        waitingForPlayer = value;
        OnWaitStateChanged?.Invoke(waitingForPlayer);
        if (waitingForPlayer) OnNeedPlayerInteract?.Invoke();
    }

    public void ReleaseAndContinue(float obstacleIgnoreSeconds = 0.25f)
    {
        SetWaiting(false);
        ignoreObstacleUntil = Time.time + obstacleIgnoreSeconds;

        if (returnAfterFix)
        {
            returningToPickup = true;
            returnAfterFix = false;
        }

        // Reset states that are "fixed"
        Slipped = false;
    }

    public void MarkReturnAfterFix()
    {
        returnAfterFix = true;
    }

    public void StopPatrol()
    {
        active = false;
        SetWaiting(false);
    }

    private void CollectPatrolPoints()
    {
        var all = FindObjectsByType<PatrolPoint>(FindObjectsSortMode.None);
        points = new Dictionary<int, PatrolPoint>();

        foreach (var p in all)
        {
            if (points.ContainsKey(p.id))
            {
                Debug.LogError($"Duplicate Patrolpoint ID {p.id} found on {p.gameObject.name}!", p);
                active = false;
                return;
            }
            points.Add(p.id, p);
        }
    }

    private void InitPrevMap()
    {
        prevMap.Clear();
        // Wird später gebraucht, aber Points sind erst nach Collect verfügbar
        // -> hier leer lassen, nach Collect aufrufen
    }

    private void BuildPrevMap()
    {
        prevMap.Clear();
        foreach (var kvp in points)
        {
            int id = kvp.Key;
            int next = kvp.Value.nextId;
            if (next != -1) prevMap[next] = id;
        }
    }

    private void AdvanceToNext()
    {
        if (points == null || points.Count == 0) return;
        if (prevMap.Count == 0) BuildPrevMap();

        int nextId;

        if (returningToPickup)
        {
            if (currentTarget != null && currentTarget.isPickupPoint)
            {
                returningToPickup = false;
                nextId = currentTarget.nextId;
            }
            else
            {
                if (!prevMap.TryGetValue(currentTarget.id, out nextId))
                {
                    Debug.LogWarning("No previous point found - stop reverse.", this);
                    returningToPickup = false;
                    nextId = currentTarget.nextId;
                }
            }
        }
        else
        {
            nextId = currentTarget.nextId;
        }

        if (nextId == -1)
        {
            StopPatrol();
            return;
        }

        if (!points.TryGetValue(nextId, out var nextPoint))
        {
            Debug.LogError($"Next Patrolpoint ID {nextId} not found!", this);
            StopPatrol();
            return;
        }

        currentTarget = nextPoint;
    }

    private void HandlePointArrival(PatrolPoint point)
    {
        if (point == null) return;

        if (point.isPickupPoint)
        {
            hasPaket = true;
            OnPickupArrived?.Invoke();
        }

        if (point.isDropOffPoint)
        {
            hasPaket = false;
        }
    }
}
