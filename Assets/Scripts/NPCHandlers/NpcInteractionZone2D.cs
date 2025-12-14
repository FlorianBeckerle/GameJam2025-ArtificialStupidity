using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcInteractionZone2D : MonoBehaviour
{
    [Header("Zone/Body")]
    [SerializeField] private Collider2D interactZone; // Trigger
    [SerializeField] private Collider2D bodyCollider; // non-trigger

    private NpcPatrolController2D controller;

    public Collider2D InteractZone => interactZone;

    void Awake()
    {
        controller = GetComponent<NpcPatrolController2D>();
        if (interactZone != null) interactZone.enabled = false;
    }

    void OnEnable()
    {
        controller.OnWaitStateChanged += OnWaitChanged;
    }

    void OnDisable()
    {
        controller.OnWaitStateChanged -= OnWaitChanged;
    }

    private void OnWaitChanged(bool waiting)
    {
        // Zone nur aktiv, wenn NPC wirklich wartet (Obstacle oder Hazard)
        if (interactZone != null) interactZone.enabled = waiting;
        if (!waiting)
        {
            controller.PlayerInZone = false;
            if (bodyCollider != null) bodyCollider.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        controller.PlayerInZone = true;
        if (bodyCollider != null) bodyCollider.enabled = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        controller.PlayerInZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        controller.PlayerInZone = false;
        if (bodyCollider != null) bodyCollider.enabled = true;
    }
}
