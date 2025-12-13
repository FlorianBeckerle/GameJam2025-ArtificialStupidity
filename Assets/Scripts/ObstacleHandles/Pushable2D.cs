using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pushable2D : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float stepSize = 1f;
    [SerializeField] private float pushSpeed = 8f;
    [SerializeField] private LayerMask blockingMask;
    [SerializeField] private KeyCode pushKey = KeyCode.E;

    private Rigidbody2D rb;
    private Collider2D bodyCollider; // der NICHT-trigger Collider (BoxCollider2D)

    private bool moving;
    private Vector2 targetPos;

    // --- "Nur die nächste Box" ---
    private static Pushable2D selected;
    private static Transform interactor; // z.B. Player

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPos = rb.position;

        // den ersten NICHT-trigger Collider als Body nehmen
        var cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            if (!c.isTrigger) { bodyCollider = c; break; }
        }

        if (bodyCollider == null)
            Debug.LogError("Pushable2D: Kein non-trigger Collider2D gefunden (z.B. BoxCollider2D)!", this);
    }

    void Update()
    {

        Debug.Log("UPDATE IS RUNNING", this);
        // nur die ausgewählte (nächste) Box reagiert
        if (selected != this) return;
        if (interactor == null) return;
        if (moving) return;

        if (!Input.GetKeyDown(pushKey)) return;

        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            dir += Vector2.up;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            dir += Vector2.down;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            dir += Vector2.left;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            dir += Vector2.right;   
        
        if (dir == Vector2.zero) return;

        TryPush(dir);
    }

    void FixedUpdate()
    {
        if (!moving) return;

        Vector2 next = Vector2.MoveTowards(rb.position, targetPos, pushSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if ((targetPos - rb.position).sqrMagnitude < 0.0001f)
        {
            rb.MovePosition(targetPos);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            moving = false;
        }
    }

    bool TryPush(Vector2 dir)
    {
        if (bodyCollider == null) return false;

        Vector2 start = rb.position;

        var hit = Physics2D.BoxCast(
            start,
            bodyCollider.bounds.size * 0.95f,
            0f,
            dir,
            stepSize,
            blockingMask
        );

        Debug.Log(hit.collider ? $"Blocked by {hit.collider.name}" : "Free", this);

        if (hit.collider != null) return false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        targetPos = start + dir * stepSize;
        moving = true;
        return true;
    }

    // Trigger-Events kommen vom CircleCollider2D (Is Trigger = true)
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player im InteractZone", this);
        if (!other.CompareTag("Player")) return;
        interactor = other.transform;
        TrySelectMe();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        interactor = other.transform;
        TrySelectMe();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (selected == this) selected = null;
        // interactor lassen wir drin; selected entscheidet sowieso
    }

    void TrySelectMe()
    {
        if (interactor == null) return;

        if (selected == null)
        {
            selected = this;
            return;
        }

        float myDist = (transform.position - interactor.position).sqrMagnitude;
        float selDist = (selected.transform.position - interactor.position).sqrMagnitude;

        if (myDist < selDist) selected = this;
    }

    Vector2 SnapToCardinal(Vector2 dir)
    {
        if (dir == Vector2.zero) return Vector2.zero;

        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return dir.x >= 0 ? Vector2.right : Vector2.left;
        else
            return dir.y >= 0 ? Vector2.up : Vector2.down;
    }
}
