using UnityEngine;

[RequireComponent(typeof(NpcPatrolController2D))]
public class NpcMinigameHandler2D : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private KeyCode startFixKey = KeyCode.F;
    [SerializeField] private KeyCode continueKey = KeyCode.E;

    [Header("UI Parent")]
    [SerializeField] private Transform uiParent;

    [Header("Shortcircuit Minigames (random)")]
    [SerializeField] private GameObject[] shortcircuitMinigamePrefabs;

    [Header("Fallback Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float interactDistance = 1.5f;

    [Header("Collision Gate")]
    [SerializeField] private MinigameCollisionGate collisionGate; // <- NEU

    private NpcPatrolController2D npc;
    private Transform playerTf;

    private INpcMinigame activeMinigame;
    private GameObject activeGO;
    private bool running;

    void Awake()
    {
        npc = GetComponent<NpcPatrolController2D>();
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) playerTf = player.transform;

        // optional: falls nicht im Inspector gesetzt
        if (!collisionGate) collisionGate = FindFirstObjectByType<MinigameCollisionGate>();
    }

    void Update()
    {
        if (npc == null || !npc.Active) return;
        if (!npc.WaitingForPlayer) return;

        bool canInteract = npc.PlayerInZone;

        if (playerTf != null)
        {
            canInteract |= Vector2.Distance(playerTf.position, transform.position) <= interactDistance;
        }

        if (!canInteract) return;

        if (npc.Shortcircuited)
        {
            if (!running && Input.GetKeyDown(startFixKey))
                StartRandomShortcircuitMinigame();
            return;
        }

        if (Input.GetKeyDown(continueKey))
            npc.ReleaseAndContinue();
    }

    private void StartRandomShortcircuitMinigame()
    {
        if (shortcircuitMinigamePrefabs == null || shortcircuitMinigamePrefabs.Length == 0) return;
        if (activeGO != null) return;

        int idx = Random.Range(0, shortcircuitMinigamePrefabs.Length);
        var prefab = shortcircuitMinigamePrefabs[idx];
        if (prefab == null) return;

        running = true;

        activeGO = Instantiate(prefab, uiParent);
        activeMinigame = activeGO.GetComponentInChildren<INpcMinigame>();

        if (activeMinigame == null)
        {
            Destroy(activeGO);
            activeGO = null;
            running = false;
            return;
        }

        // <<< WICHTIG: Colliders der Main Scene ausschalten, sobald Minigame da ist
        if (collisionGate) collisionGate.Enter(activeGO.transform);

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
        Cleanup(keepNpcWaiting: true);
        npc.SetWaiting(true);
    }

    private void Cleanup(bool keepNpcWaiting = false)
    {
        if (activeMinigame != null)
        {
            activeMinigame.Solved -= OnSolved;
            activeMinigame.Failed -= OnFailed;
        }

        // <<< Revert Colliders zuerst (wichtig, falls Destroy noch Frame braucht)
        if (collisionGate) collisionGate.Exit();

        if (activeGO != null) Destroy(activeGO);

        activeMinigame = null;
        activeGO = null;
        running = false;

        if (!keepNpcWaiting)
            npc.SetWaiting(false);
    }
}
