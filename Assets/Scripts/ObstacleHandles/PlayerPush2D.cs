using Unity.VisualScripting;
using UnityEngine;

public class PlayerPush2D : MonoBehaviour
{
    [SerializeField] private KeyCode pushKey = KeyCode.E;
    [SerializeField] private float interactRadius = 1.1f;
    [SerializeField] private LayerMask pushableMask;

    void Update()
    {
        if (!Input.GetKeyDown(pushKey)) return;

        var hit = Physics2D.OverlapCircle(transform.position, interactRadius, pushableMask);
        if (hit == null) return;

        var pushable = hit.GetComponent<Pushable2D>();
        if (pushable == null) return;

        Vector2 toBox = (hit.transform.position - transform.position);
        pushable.TryPush(toBox);
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
