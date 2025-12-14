using UnityEngine;

public class NpcVisuals2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private NpcPatrolById2D npc;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    void Reset()
    {
        if (npc == null) npc = GetComponentInParent<NpcPatrolById2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (npc == null || animator == null) return;

       
        bool falls     = npc.IsSlipped;
        bool isTalking = npc.IsWaitingForPlayer;
        bool isWalking = npc.IsMoving;
        bool hasPaket  = npc.HasPaket;
        bool hasError  = npc.IsShortcircuited && !falls;

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

    void OnEnable()
    {
        if (npc == null)
            npc = GetComponentInParent<NpcPatrolById2D>();

        if (npc != null)
            npc.OnPickupArrived += TriggerPickup;
    }

    void OnDisable()
    {
        if (npc != null)
            npc.OnPickupArrived -= TriggerPickup;
    }

    void TriggerPickup()
    {
        if (animator != null)
            animator.SetTrigger("pickUp");
    }
}
