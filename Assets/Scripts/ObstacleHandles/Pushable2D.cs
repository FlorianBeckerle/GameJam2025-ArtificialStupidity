using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Pushable2D : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float stepSize = 1f;
    [SerializeField] private float pushSpeed = 8f;
    [SerializeField] private float interactRadius = 1.1f;

    [SerializeField] private LayerMask blockingMask;   // Walls + Obstacle
    [SerializeField] private LayerMask interactorMask; // Player / NPC

    [SerializeField] private KeyCode pushKey = KeyCode.E;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool moving;
    private Vector2 targetPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        targetPos = rb.position;
    }

    void Update()
    {
        if (moving) return;

        if (!Input.GetKeyDown(pushKey)) return;

        // Prüfen: Ist ein Interactor nah genug?
        var interactor = Physics2D.OverlapCircle(
            rb.position,
            interactRadius,
            interactorMask
        );

        if (interactor == null) return;

        // Richtung aus Input holen
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        Vector2 dir = SnapToCardinal(input);
        if (dir == Vector2.zero) return;

        TryPush(dir);
    }

    void FixedUpdate()
    {
        if (!moving) return;

        Vector2 next = Vector2.MoveTowards(
            rb.position,
            targetPos,
            pushSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(next);

        if ((targetPos - rb.position).sqrMagnitude < 0.0001f)
        {
            rb.MovePosition(targetPos);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            moving = false;
        }
    }

    void TryPush(Vector2 dir)
    {
        Vector2 start = rb.position;
        Vector2 dest = start + dir * stepSize;

        // Blockiert?
        var hit = Physics2D.BoxCast(
            start,
            col.bounds.size * 0.95f,
            0f,
            dir,
            stepSize,
            blockingMask
        );

        if (hit.collider != null) return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        targetPos = dest;
        moving = true;
    }

    Vector2 SnapToCardinal(Vector2 dir)
    {
        if (dir == Vector2.zero) return Vector2.zero;

        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return dir.x >= 0 ? Vector2.right : Vector2.left;
        else
            return dir.y >= 0 ? Vector2.up : Vector2.down;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
