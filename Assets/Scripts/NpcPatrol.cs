using UnityEngine;

public class NpcPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reachDistance = 0.1f;

    private Transform target;

    void Start()
    {
        target = pointB;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) <= reachDistance)
        {
            OnReachedTarget();
        }
    }

    void OnReachedTarget()
    {
        if (target == pointB)
        {
            ScoreManager.Instance.Add(1);
        }

        // Ziel wechseln
        target = (target == pointA) ? pointB : pointA;

        // Optional umdrehen
        transform.Rotate(0f, 180f, 0f);
    }
}
