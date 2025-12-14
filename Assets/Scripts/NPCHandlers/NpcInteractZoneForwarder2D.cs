using UnityEngine;

public class NpcInteractZoneForwarder2D : MonoBehaviour
{
    private NpcPatrolById2D npc;

    void Awake()
    {
        npc = GetComponentInParent<NpcPatrolById2D>();
        Debug.Log("FORWARDER: Awake, found npc: " + (npc != null), this);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("FORWARDER: Player entered interact zone" + other.name, this);
        npc?.OnZoneEnter(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        npc?.OnZoneExit(other);
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        npc?.OnZoneStay(other);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
}
