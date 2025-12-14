using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcMinigameHandler2D : MonoBehaviour
{
    // -------------------------
    // Keys
    // -------------------------
    [Header("Interaction Keys")]
    [SerializeField] private KeyCode fixKey = KeyCode.F;      // Minigame starten / fixen
    [SerializeField] private KeyCode continueKey = KeyCode.E; // Ohne Minigame weiterlaufen

    // -------------------------
    // Minigames
    // -------------------------
    [Header("Minigames")]
    [SerializeField] private PipeManager pipeMinigamePrefab;
    [SerializeField] private Transform uiParent;

    private NpcPatrolController2D npc;
    private PipeManager activePipeMinigame;
    private bool minigameRunning;

    // -------------------------
    // Unity
    // -------------------------
    void Awake()
    {
        npc = GetComponent<NpcPatrolController2D>();
    }

    void Update()
    {
        if (npc == null || !npc.Active) return;
        if (!npc.WaitingForPlayer) return;
        if (!npc.PlayerInZone) return;

        // Kurzschluss → Minigame
        if (npc.Shortcircuited)
        {
            if (!minigameRunning && Input.GetKeyDown(fixKey))
            {
                StartPipeMinigame();
            }
            return;
        }

        // Andere Wartezustände (Obstacle / Oil)
        if (Input.GetKeyDown(continueKey))
        {
            npc.ReleaseAndContinue();
        }
    }

    // -------------------------
    // Pipe Minigame
    // -------------------------
    private void StartPipeMinigame()
    {
        if (pipeMinigamePrefab == null)
        {
            Debug.LogWarning("No Pipe Minigame prefab assigned!", this);
            return;
        }

        if (activePipeMinigame != null) return;

        minigameRunning = true;

        activePipeMinigame = Instantiate(pipeMinigamePrefab, uiParent);
        activePipeMinigame.Solved += OnPipeSolved;
        activePipeMinigame.Failed += OnPipeFailed;
    }

    private void OnPipeSolved()
    {
        CleanupPipeMinigame();

        npc.Shortcircuited = false;
        npc.ReleaseAndContinue();

        minigameRunning = false;
    }

    private void OnPipeFailed()
    {
        // NPC bleibt im Fehlerzustand
        minigameRunning = false;
    }

    private void CleanupPipeMinigame()
    {
        if (activePipeMinigame == null) return;

        activePipeMinigame.Solved -= OnPipeSolved;
        activePipeMinigame.Failed -= OnPipeFailed;
        Destroy(activePipeMinigame.gameObject);
        activePipeMinigame = null;
    }
}
