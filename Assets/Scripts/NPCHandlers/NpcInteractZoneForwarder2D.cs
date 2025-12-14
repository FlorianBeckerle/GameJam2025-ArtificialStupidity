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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("FORWARDER: PLAYER entered NPC zone", this);
        npc?.OnZoneEnter(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        npc?.OnZoneStay(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        npc?.OnZoneExit(other);
    }
}
