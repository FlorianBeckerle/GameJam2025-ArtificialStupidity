using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LevelSectionChaser2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LevelSectionMinigameAdapter adapter;
    [SerializeField] private string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float catchupPerSecond = 0.15f;
    [SerializeField] private float minXOffset = 8f;

    private Transform player;
    private Rigidbody2D rb;

    private Vector2 startPosition;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        startPosition = rb.position;
        currentSpeed = speed;

        if (adapter == null)
            adapter = GetComponentInParent<LevelSectionMinigameAdapter>();

        var p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
        if (adapter == null || player == null) return;
        if (!adapter.IsRunning) return;

        currentSpeed += catchupPerSecond * Time.fixedDeltaTime;

        Vector2 pos = rb.position;
        pos.x += currentSpeed * Time.fixedDeltaTime;

        float targetMinX = player.position.x - minXOffset;
        if (pos.x < targetMinX) pos.x = targetMinX;

        rb.MovePosition(pos);
    }

    public void ResetChaser()
    {
        currentSpeed = speed;
        rb.position = startPosition;
        Physics2D.SyncTransforms();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if(adapter == null || !adapter.TriggersArmed) return;
        adapter.Fail();
    }
}
