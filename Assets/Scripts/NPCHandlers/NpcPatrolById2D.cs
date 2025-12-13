using UnityEngine;
using System.Collections.Generic;
using System.Data;

public class NpcPatrolById2D : MonoBehaviour
{
    [SerializeField] private int startId = 0;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private SpriteRenderer sprite;

    private Dictionary<int, PatrolPoint> points;
    private PatrolPoint currentTarget;
    private bool active = true;


    void Awake()
    {
        var all = FindObjectsByType<PatrolPoint>(FindObjectsSortMode.None);
        points = new Dictionary<int, PatrolPoint>();

        foreach (var p in all)
        {
            if (points.ContainsKey(p.id))
            {
                Debug.LogError($"Duplicate Patrolpoint ID {p.id} found on {p.gameObject.name}!", p);
                active = false;
                return;
    
            }
            points.Add(p.id, p);
        }

        if(!points.TryGetValue(startId, out currentTarget))
        {
            Debug.LogError($"Starting Patrolpoint ID {startId} not found!", this);
            active = false;
            return;
        }

        transform.position = currentTarget.transform.position;
        AdvanceToNext();

    }

    void Update()
    {
        if(!active || currentTarget == null) return;

        Vector3 targetPos = currentTarget.transform.position;
        Vector3 next = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.position = next;

        if(Vector2.Distance(transform.position, targetPos) <= reachDistance )
        {
            AdvanceToNext();
        }
    }

    void AdvanceToNext()
    {
        int nextId = currentTarget.nextId;

        if(nextId == -1)
        {
            active = false;
            return;
        }

        if (!points.TryGetValue(nextId, out var nextPoint))
        {
            Debug.LogError($"Next Patrolpoint ID {nextId} not found!", this);
            active = false;
            return;
        }

        if (sprite != null)
        {
            float dx = nextPoint.transform.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.001f) sprite.flipX = (dx < 0f);
        }

        currentTarget = nextPoint;
    }
}
