using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcMinigameHandler2D : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private KeyCode startFixKey = KeyCode.F;      // Minigame starten
    [SerializeField] private KeyCode continueKey = KeyCode.E;      // nur für non-shortcircuit waits

    [Header("UI Parent")]
    [SerializeField] private Transform uiParent;

    [Header("Shortcircuit Minigames (random)")]
    [SerializeField] private GameObject[] shortcircuitMinigamePrefabs;

    private NpcPatrolController2D npc;

    private INpcMinigame activeMinigame;
    private GameObject activeGO;
    private bool running;

    void Awake()
    {
        npc = GetComponent<NpcPatrolController2D>();
    }

    void Update()
    {
        if (npc == null || !npc.Active) return;

        // nur wenn NPC wartet + Player in Range
        if (!npc.WaitingForPlayer || !npc.PlayerInZone) return;

        // SHORTCIRCUIT => random Minigame starten
        if (npc.Shortcircuited)
        {
            if (!running && Input.GetKeyDown(startFixKey))
                StartRandomShortcircuitMinigame();

            return;
        }

        // andere Wait States (Obstacle / Oil) => optional weiter mit E
        if (Input.GetKeyDown(continueKey))
            npc.ReleaseAndContinue();
    }

    private void StartRandomShortcircuitMinigame()
    {
        if (shortcircuitMinigamePrefabs == null || shortcircuitMinigamePrefabs.Length == 0)
        {
            Debug.LogWarning("No shortcircuit minigame prefabs assigned!", this);
            return;
        }

        if (activeGO != null) return;

        // Random wählen
        int idx = Random.Range(0, shortcircuitMinigamePrefabs.Length);
        GameObject prefab = shortcircuitMinigamePrefabs[idx];

        if (prefab == null)
        {
            Debug.LogWarning($"Shortcircuit minigame prefab at index {idx} is null!", this);
            return;
        }

        StartMinigame(prefab);
    }

    private void StartMinigame(GameObject prefab)
    {
        running = true;

        activeGO = Instantiate(prefab, uiParent);
        activeMinigame = activeGO.GetComponentInChildren<INpcMinigame>();

        if (activeMinigame == null)
        {
            Debug.LogError($"Prefab '{prefab.name}' has no component implementing INpcMinigame.", this);
            Destroy(activeGO);
            activeGO = null;
            running = false;
            return;
        }

        activeMinigame.Solved += OnSolved;
        activeMinigame.Failed += OnFailed;
        activeMinigame.Begin();
    }

    private void OnSolved()
    {
        Cleanup();

        npc.Shortcircuited = false;
        npc.ReleaseAndContinue();
    }

    private void OnFailed()
    {
        // NPC bleibt shortcircuited, Player kann erneut F drücken
        Cleanup(keepNpcWaiting: true);
    }

    private void Cleanup(bool keepNpcWaiting = false)
    {
        if (activeMinigame != null)
        {
            activeMinigame.Solved -= OnSolved;
            activeMinigame.Failed -= OnFailed;
        }

        if (activeGO != null)
            Destroy(activeGO);

        activeMinigame = null;
        activeGO = null;
        running = false;

        if (!keepNpcWaiting)
            npc.SetWaiting(false);
    }
}
