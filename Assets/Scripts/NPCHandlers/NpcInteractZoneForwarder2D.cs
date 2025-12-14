using UnityEngine;

public class NpcInteractZoneForwarder2D : MonoBehaviour
{
    private NpcPatrolById2D npc;
    private Collider2D zone;

    void Awake()
    {
        npc = GetComponentInParent<NpcPatrolById2D>();
        zone = GetComponent<Collider2D>();

        // ⭐ Zone beim Start sicher AUS, damit es kein Start-Enter/Exit spamt
        // if (zone != null) zone.enabled = false;

        Debug.Log($"FORWARDER: Awake, found npc: {npc != null}", this);
    }


}
