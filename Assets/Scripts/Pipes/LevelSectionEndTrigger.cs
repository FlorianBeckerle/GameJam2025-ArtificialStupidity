using UnityEngine;

public class LevelSectionEndTrigger : MonoBehaviour
{
    [SerializeField] private LevelSectionMinigameAdapter adapter;
    [SerializeField] private string playerTag = "Player";

    void Awake()
    {
        if (adapter == null) adapter = GetComponentInParent<LevelSectionMinigameAdapter>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (adapter == null || !adapter.TriggersArmed) return;
        adapter.Complete();

    }
}
