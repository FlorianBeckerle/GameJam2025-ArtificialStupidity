using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcObstacleStopper2D : MonoBehaviour
{
    [Header("Obstacle Stop")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float stopDistance = 0.3f;
    [SerializeField] private float rayOffsetY = 0.0f;

    private NpcPatrolController2D controller;

    void Awake()
    {
        controller = GetComponent<NpcPatrolController2D>();
    }

    void Update()
    {
        if (!controller.Active) return;
        if (!controller.IsMoving) return;

        if (Time.time < controller.IgnoreObstacleUntil) return;
        if (obstacleMask == 0) return;

        Vector2 dir = controller.MoveDirection;
        if (dir == Vector2.zero) return;

        Vector2 origin = controller.RB.position + new Vector2(0f, rayOffsetY);
        var hit = Physics2D.Raycast(origin, dir, stopDistance, obstacleMask);

        if (hit.collider != null)
        {
            controller.SetWaiting(true);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (controller == null) controller = GetComponent<NpcPatrolController2D>();

        Vector2 dir = controller.MoveDirection;
        if (dir == Vector2.zero) return;

        Gizmos.DrawLine(
            (Vector3)controller.RB.position,
            (Vector3)controller.RB.position + (Vector3)(dir * stopDistance)
        );
    }
}
