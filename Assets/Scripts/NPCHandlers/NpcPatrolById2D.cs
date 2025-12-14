using UnityEngine;
using System.Collections.Generic;

//
// NpcPatrolById2D
// - Patrouilliert über PatrolPoints (per ID-Kette)
// - Stoppt vor Obstacles via Raycast und wartet auf Player (Key E)
// - Wird bei "Puddle" kurzgeschlossen (Trigger), wartet auf Fix durch Player (Key F)
// - NPC ist KINEMATIC => wird nicht von Physik/Player geschoben, bewegt sich nur über rb.MovePosition
//
// Wichtig im Inspector:
// - bodyCollider      = Capsule/BoxCollider2D (kein Trigger) am NPC
// - interactZone      = CircleCollider2D (Is Trigger = true) am NPC (für Player-Nähe)
// - obstacleMask      = NUR Obstacle-Layer (nicht Puddle/Hazards)
// - Puddle GameObjects müssen Tag "Puddle" haben und einen Trigger-Collider
//
[RequireComponent(typeof(Rigidbody2D))]
public class NpcPatrolById2D : MonoBehaviour
{
    // -------------------------
    // Patrol Settings
    // -------------------------
    [Header("Patrol")]
    [SerializeField] private int startId = 0;            // Start-PatrolPoint ID
    [SerializeField] private float speed = 2f;           // Bewegungs-Speed (Einheiten/Sekunde)
    [SerializeField] private float reachDistance = 0.1f; // Wie nah muss NPC am Ziel sein, um zum nächsten Punkt zu wechseln
    [SerializeField] private SpriteRenderer sprite;      // Optional: zum Flippen links/rechts

    // -------------------------
    // Obstacle Stop (Raycast)
    // -------------------------
    [Header("Obstacle Stop")]
    [SerializeField] private LayerMask obstacleMask;     // Nur Obstacle-Layer! (Kisten/Wände)
    [SerializeField] private float stopDistance = 0.3f;  // Raycast-Länge: wie früh vor dem Hindernis stoppen
    [SerializeField] private float rayOffsetY = 0.0f;    // Raycast Start leicht hoch/runter verschieben (falls nötig)
    [SerializeField] private Collider2D interactZone;    // CircleCollider2D Trigger: Player ist "in der Nähe" (wird an/aus geschaltet)

    // -------------------------
    // Player Interaction
    // -------------------------
    [Header("Player Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Key für "Obstacle warten -> weiterlaufen"

    // Body Collider (Physik)
    [SerializeField] private Collider2D bodyCollider;    // NPC-Body Collider (kein Trigger). Wird disabled, wenn Player nahe ist, damit Player ihn nicht schiebt.

    // -------------------------
    // Water Puddle (Hazard)
    // -------------------------
    [Header("Water Puddle (shortcircuit)")]
    [SerializeField] private KeyCode puddleKey = KeyCode.F;   // Placeholder "Minigame bestanden"
    private bool shortcircuited = false;                       // NPC ist kurzgeschlossen und wartet auf Fix

    private bool slipped = false;                            // NPC ist in Oil ausgerutscht
    // -------------------------
    // Internals
    // -------------------------
    private Dictionary<int, PatrolPoint> points;          // Alle PatrolPoints nach ID
    private PatrolPoint currentTarget;                    // Aktuelles Ziel (der Punkt, zu dem gelaufen wird)
    private Rigidbody2D rb;

    private bool active = true;                           // Patrouille aktiv?
    private bool waitingForPlayer = false;                // Wartet der NPC wegen Obstacle auf Player (E)?
    private bool playerInZone = false;                    // Player ist in InteractZone (Circle Trigger)
    private float ignoreObstacleUntil = 0f;               // Kurz Obstacles ignorieren (damit er nach "E/F" nicht sofort wieder stoppt)

    void Awake()
    {
        // InteractZone am Anfang deaktivieren, damit der NPC nicht sofort Interaktions-Trigger spamt.
        // Wird nur aktiviert, wenn NPC wirklich wartet (Obstacle oder Shortcircuit).
        if (interactZone != null) interactZone.enabled = false;

        // Sicherheits-Resets
        playerInZone = false;
        waitingForPlayer = false;

        // Rigidbody Setup: Kinematic => NPC wird nicht geschoben, nur durch Script bewegt
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // PatrolPoints einsammeln (alle PatrolPoint-Komponenten in der Szene)
        var all = FindObjectsByType<PatrolPoint>(FindObjectsSortMode.None);
        points = new Dictionary<int, PatrolPoint>();

        foreach (var p in all)
        {
            // Duplicate-IDs verhindern
            if (points.ContainsKey(p.id))
            {
                Debug.LogError($"Duplicate Patrolpoint ID {p.id} found on {p.gameObject.name}!", p);
                active = false;
                return;
            }
            points.Add(p.id, p);
        }

        // Startpunkt finden
        if (!points.TryGetValue(startId, out currentTarget))
        {
            Debug.LogError($"Starting Patrolpoint ID {startId} not found!", this);
            active = false;
            return;
        }

        // NPC auf Startpunkt setzen und direkt zum nächsten Ziel wechseln
        rb.position = currentTarget.transform.position;
        AdvanceToNext();
    }

    void Update()
    {
        // -------------------------
        // 1) Kurzschluss-Zustand: NPC steht und wartet auf "Fix" (Key F)
        // -------------------------
        if (shortcircuited || slipped)
        {
            waitingForPlayer = true;
        }

        // -------------------------
        // 2) Wenn Patrol deaktiviert oder kein Ziel vorhanden -> raus
        // -------------------------
        if (!active || currentTarget == null) return;

        // -------------------------
        // 3) Wartestatus wegen Obstacle: NPC steht und wartet auf Player (Key E)
        // -------------------------
        if (waitingForPlayer)
        {
            Debug.Log($"WAITING | playerInZone={playerInZone}", this);

            rb.linearVelocity = Vector2.zero;

            // Player muss in InteractZone sein UND E drücken
            if (playerInZone && Input.GetKeyDown(interactKey))
            {
                waitingForPlayer = false;

                // Kurz Obstacles ignorieren, damit er nach dem "Go" nicht sofort wieder stoppt
                ignoreObstacleUntil = Time.time + 0.25f;

                // InteractZone wieder aus (damit es nicht dauernd triggert)
                if (interactZone != null)
                    interactZone.enabled = false;

                // Nähezustand zurücksetzen
                playerInZone = false;

                shortcircuited = false;
            }

            // In Wartestatus nicht weiter patrouillieren
            return;
        }

        // -------------------------
        // 4) Normaler Patrol-Mode: zum currentTarget laufen
        // -------------------------
        Vector2 targetPos = currentTarget.transform.position;
        Vector2 dir = (targetPos - rb.position).normalized;

        // 4a) Obstacle Raycast: nur gegen obstacleMask
        // obstacleMask != 0 => falls im Inspector nichts gesetzt ist, vermeiden wir nutzlosen Raycast
        if (Time.time >= ignoreObstacleUntil && obstacleMask != 0)
        {
            Vector2 rayOrigin = rb.position + new Vector2(0f, rayOffsetY);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, dir, stopDistance, obstacleMask);

            // Wenn ein Obstacle vor uns ist -> warten
            if (hit.collider != null)
            {
                waitingForPlayer = true;
                rb.linearVelocity = Vector2.zero;

                // InteractZone aktivieren, damit Player "E" drücken kann
                if (interactZone != null)
                    interactZone.enabled = true;

                return;
            }
        }

        // 4b) Bewegung zum Ziel
        Vector2 next = Vector2.MoveTowards(rb.position, targetPos, speed * Time.deltaTime);
        rb.MovePosition(next);

        // 4c) Ziel erreicht -> nächsten PatrolPoint setzen
        if (Vector2.Distance(rb.position, targetPos) <= reachDistance)
            AdvanceToNext();
    }

    // Wechselt currentTarget auf den nächsten PatrolPoint in der Kette
    void AdvanceToNext()
    {
        int nextId = currentTarget.nextId;

        // -1 bedeutet "Ende"
        if (nextId == -1)
        {
            active = false;
            return;
        }

        // nächsten PatrolPoint holen
        if (!points.TryGetValue(nextId, out var nextPoint))
        {
            Debug.LogError($"Next Patrolpoint ID {nextId} not found!", this);
            active = false;
            return;
        }

        // Sprite Flip links/rechts je nach Richtung
        if (sprite != null)
        {
            float dx = nextPoint.transform.position.x - rb.position.x;
            if (Mathf.Abs(dx) > 0.001f)
                sprite.flipX = (dx < 0f);
        }

        currentTarget = nextPoint;
    }

    // Nur zum Debug: zeigt Raycast-Linie im Scene View wenn Objekt ausgewählt ist
    void OnDrawGizmosSelected()
    {
        // Hinweis: rb ist in Edit-Mode meist null -> dann nichts zeichnen
        if (rb == null || currentTarget == null) return;

        Gizmos.DrawLine(
            (Vector3)rb.position,
            (Vector3)rb.position + (Vector3)((((Vector2)currentTarget.transform.position - rb.position).normalized) * stopDistance)
        );
    }

    // -------------------------
    // Trigger Handling
    // - Puddle: Hazard -> Kurzschluss auslösen und Puddle verschwinden lassen
    // - Player: InteractZone -> playerInZone setzen, bodyCollider deaktivieren (nicht schiebbar)
    // -------------------------

    // Player kommt in Zone -> markieren + BodyCollider aus, damit Player NPC nicht schiebt
    public void OnZoneEnter(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = true;

        if (bodyCollider != null)
            bodyCollider.enabled = false;

        Debug.Log("PLAYER entered NPC zone", this);
    }

    // Player geht raus -> unmarkieren + BodyCollider wieder an
    public void OnZoneExit(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;

        if (bodyCollider != null)
            bodyCollider.enabled = true;

        Debug.Log("PLAYER exited NPC zone", this);
    }

    // Player bleibt drin -> sicherstellen, dass playerInZone true bleibt
    public void OnZoneStay(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = true;
        // Debug.Log("PLAYER staying in NPC zone", this); // kann sehr viel spammen -> optional
    }

    // Unity Trigger: ein Trigger-Collider am NPC (z.B. Circle) berührt einen anderen Collider
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) PUDDLE: NPC läuft in Hazard rein (Trigger auf dem Puddle)
        if (other.CompareTag("Puddle"))
        {
            shortcircuited = true;
            rb.linearVelocity = Vector2.zero;

            // InteractZone aktivieren, damit Player zum Fixen in die Nähe gehen kann
            if (interactZone != null)
                interactZone.enabled = true;

            // Puddle "verbrauchen": verschwindet sofort
            Destroy(other.gameObject);

            Debug.Log("NPC shortcircuited by puddle!", this);
            return;
        }

        // 2) PLAYER: Player ist in InteractZone
        if (other.CompareTag("Player"))
        {
            OnZoneEnter(other);
            return;
        }

        if (other.CompareTag("Oil"))
        {
            slipped = true;
            rb.linearVelocity = Vector2.zero;

            // InteractZone aktivieren, damit Player zum Fixen in die Nähe gehen kann
            if (interactZone != null)
                interactZone.enabled = true;

            Debug.Log("NPC slipped on oil!", this);
            return;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Nur Player-Interaktion dauerhaft updaten
        if (other.CompareTag("Player"))
            OnZoneStay(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Nur Player-Interaktion beim Verlassen
        if (other.CompareTag("Player"))
            OnZoneExit(other);
    }
}
