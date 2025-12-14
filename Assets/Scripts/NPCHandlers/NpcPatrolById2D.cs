using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D))]
public class NpcPatrolById2D : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private int startId = 0;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Obstacle Stop")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float stopDistance = 0.3f;
    [SerializeField] private float rayOffsetY = 0.0f;

    [Header("Player Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private Dictionary<int, PatrolPoint> points;
    private PatrolPoint currentTarget;
    private Rigidbody2D rb;

    private bool active = true;
    private bool waitingForPlayer = false;
    private bool playerInZone = false;
    private float ignoreObstacleUntil = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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

        if (!points.TryGetValue(startId, out currentTarget))
        {
            Debug.LogError($"Starting Patrolpoint ID {startId} not found!", this);
            active = false;
            return;
        }

        rb.position = currentTarget.transform.position;
        AdvanceToNext();
    }

    void Update()
    {
        if (!active || currentTarget == null) return;

        // ⭐ Wenn NPC wartet: nur per Player-Input weitermachen
        if (waitingForPlayer)
        {

            Debug.Log($"WAITING | playerInZone={playerInZone}", this);
            if (playerInZone && Input.GetKeyDown(interactKey)) Debug.Log("E DETECTED", this);

            rb.linearVelocity = Vector2.zero;
            if (playerInZone && Input.GetKeyDown(interactKey))
            {
                waitingForPlayer = false;
                ignoreObstacleUntil = Time.time + 0.25f; // kurz Hindernisse ignorieren
            }
            return;
        }

        Vector2 targetPos = currentTarget.transform.position;
        Vector2 dir = (targetPos - rb.position).normalized;

        if (Time.time >= ignoreObstacleUntil){
            Vector2 rayOrigin = rb.position + new Vector2(0f, rayOffsetY);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dir, stopDistance, obstacleMask);

            if (hit.collider != null)
            {
                waitingForPlayer = true;
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        Vector2 next = Vector2.MoveTowards(rb.position, targetPos, speed * Time.deltaTime);
        rb.MovePosition(next);

        if (Vector2.Distance(rb.position, targetPos) <= reachDistance)
            AdvanceToNext();
    }

    void AdvanceToNext()
    {
        int nextId = currentTarget.nextId;

        if (nextId == -1) { active = false; return; }

        if (!points.TryGetValue(nextId, out var nextPoint))
        {
            Debug.LogError($"Next Patrolpoint ID {nextId} not found!", this);
            active = false;
            return;
        }

        if (sprite != null)
        {
            float dx = nextPoint.transform.position.x - rb.position.x;
            if (Mathf.Abs(dx) > 0.001f) sprite.flipX = (dx < 0f);
        }

        currentTarget = nextPoint;
    }

    // Trigger kommt von deinem Child-CircleCollider2D (Is Trigger = ON)


    public bool IsWaiting() => waitingForPlayer;

    void OnDrawGizmosSelected()
    {
        if (rb == null || currentTarget == null) return;
        Gizmos.DrawLine((Vector3)rb.position,
            (Vector3)rb.position + (Vector3)((((Vector2)currentTarget.transform.position - rb.position).normalized) * stopDistance));
    }



    public void OnZoneEnter(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    public void OnZoneExit(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }

    public void OnZoneStay(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }
}
