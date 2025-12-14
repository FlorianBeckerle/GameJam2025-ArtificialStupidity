using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcVisuals2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    private NpcPatrolController2D npc;

    void Awake()
    {
        npc = GetComponent<NpcPatrolController2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (npc != null) npc.OnPickupArrived += TriggerPickup;
    }

    void OnDisable()
    {
        if (npc != null) npc.OnPickupArrived -= TriggerPickup;
    }

    void Update()
    {
        if (npc == null || animator == null) return;

        bool falls     = npc.Slipped;
        bool isTalking = npc.WaitingForPlayer;
        bool isWalking = npc.IsMoving;
        bool hasPaket  = npc.HasPaket;
        bool hasError  = npc.Shortcircuited && !falls;

        if (hasError || falls)
        {
            isWalking = false;
            isTalking = false;
        }

        animator.SetBool("fall", falls);
        animator.SetBool("hasError", hasError);
        animator.SetBool("isTalking", isTalking);
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("hasPaket", hasPaket);
    }

    private void TriggerPickup()
    {
        if (animator != null)
            animator.SetTrigger("pickUp");
    }
}
