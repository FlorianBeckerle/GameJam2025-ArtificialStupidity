using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcHazardResponder2D : MonoBehaviour
{
    private NpcPatrolController2D controller;

    void Awake()
    {
        controller = GetComponent<NpcPatrolController2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // PUDDLE
        if (other.CompareTag("Puddle"))
        {
            controller.Shortcircuited = true;
            controller.Slipped = false;
            controller.SetWaiting(true);
            controller.RB.linearVelocity = Vector2.zero;

            Destroy(other.gameObject);
            return;
        }

        // OIL
        if (other.CompareTag("Oil"))
        {
            controller.Slipped = true;
            controller.Shortcircuited = false;

            controller.MarkReturnAfterFix();
            controller.SetWaiting(true);
            controller.RB.linearVelocity = Vector2.zero;

            Destroy(other.gameObject);
            return;
        }
    }
}
