using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class Pushable2D : MonoBehaviour
{
    [SerializeField] private float stepSize = 1f;
    [SerializeField] private float pushSpeed = 8f;
    [SerializeField] private LayerMask blockingMask;

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
    
    void FixedUpdate()
    {
        if(!moving) return;

        Vector2 next = Vector2.MoveTowards(rb.position, targetPos, pushSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if((targetPos - rb.position).sqrMagnitude < 0.001f)
        {
            rb.MovePosition(targetPos);
            moving = false;
        }
    }

    public bool TryPush(Vector2 cardinalDir)
    {
        if(moving) return false;

        cardinalDir = SnapToCardinal(cardinalDir);
        if(cardinalDir == Vector2.zero) return false;

        Vector2 start = rb.position;
        Vector2 dest = start + cardinalDir * stepSize;

        var hit = Physics2D.BoxCast(start, col.bounds.size * 0.95f, 0f, cardinalDir, stepSize, blockingMask);

        if (hit.collider != null)
        {
            return false;
        }

        targetPos = dest;
        moving = true;
        return true;
    }

    private Vector2 SnapToCardinal(Vector2 dir)
    {
        if(dir == Vector2.zero) return Vector2.zero;
        if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return new Vector2(Mathf.Sign(dir.x), 0f);
        }
        else
        {
            return new Vector2(0f, Mathf.Sign(dir.y));
        }
    }
}
