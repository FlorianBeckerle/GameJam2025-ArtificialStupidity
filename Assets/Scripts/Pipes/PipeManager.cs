using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pipes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PipeManager : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField]
    private float maxTimer = 10f; //X seconds to solve puzzle

    private float maxWidth = 3.35f;
    
    [SerializeField]
    private GameObject timerBar;
    
    [Header("Grid")]
    [SerializeField] private Grid grid;
    
    [Header("Pipe Prefabs")]
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject pipeStraightPrefab;
    [SerializeField] private GameObject pipeCurvePrefab;            // L
    [SerializeField] private GameObject pipeCurveMirroredPrefab;    // R
    [SerializeField] private GameObject pipeStart;                  // s
    [SerializeField] private GameObject pipeEnd;                    // e
    [SerializeField] private GameObject pipeBadEnd;                 // E

    [Header("Layout (0 = empty)")]
    [TextArea(8, 20)]
    [SerializeField] private string layout = 
@"0000LrrL000000
0000u00d0000e0
0000u00Lrrre00
0srrL00000000
0000d00000LE00
0000LrrL00u000
0000000d00u0e0
0000000LrrL000";

    [Header("Generated Nodes")]
    [SerializeField] private Dictionary<Vector2Int, PipeNode> pipes = new Dictionary<Vector2Int, PipeNode>();

    [Header("Options")]
    [SerializeField] private bool clearChildrenBeforeGenerate = true;
    [SerializeField] private bool generateOnStart = true;
    void Awake()
    {
        if (!grid) grid = GetComponent<Grid>();
    }

    void Start()
    {
        if (generateOnStart)
            GenerateFromLayout();
        
        
    }

    [ContextMenu("Generate From Layout")]
    public void GenerateFromLayout()
    {
        if (!grid)
        {
            Debug.LogError("[PipeManager] No Grid assigned/found.");
            return;
        }

        if (clearChildrenBeforeGenerate)
            ClearChildren();

        pipes.Clear();

        // Split into rows, ignore empty lines
        string[] rows = layout.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (rows.Length == 0)
        {
            Debug.LogError("[PipeManager] Layout is empty.");
            return;
        }

        int height = rows.Length;
        int width = rows[0].Length;
        
        //Instantiate Background
        Vector3 worldPos = this.transform.position + new Vector3(width/2f, height/2f ,0f);
        GameObject bg = Instantiate(background, worldPos, Quaternion.Euler(0f,0f,0f), transform);
        timerBar = bg.transform.Find("TimerBar").gameObject;
        StartCoroutine(Timer());

        // Validate consistent row widths
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].Length != width)
            {
                Debug.LogError($"[PipeManager] Layout row {i} has length {rows[i].Length}, expected {width}. Fix your ASCII map.");
                return;
            }
        }

        // Your layout is written top->bottom; Unity grid Y usually bottom->top
        // So we invert Y: first row becomes top row
        for (int row = 0; row < height; row++)
        {
            int y = (height - 1) - row; // invert so first string row is top
            for (int x = 0; x < width; x++)
            {
                char c = rows[row][x];
                SpawnTileFromChar(c, new Vector3Int(x, y, 0));
            }
        }
    }

    private void SpawnTileFromChar(char c, Vector3Int cellPos)
    {
        if (c == '0') return; // empty

        GameObject prefab = null;
        Quaternion rotation = Quaternion.identity;

        switch (c)
        {
            // Straight pipes with explicit direction
            case 'u':
                prefab = pipeStraightPrefab;
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 'r':
                prefab = pipeStraightPrefab;
                rotation = Quaternion.Euler(0, 0, -90);
                break;
            case 'd':
                prefab = pipeStraightPrefab;
                rotation = Quaternion.Euler(0, 0, -180);
                break;
            case 'l':
                prefab = pipeStraightPrefab;
                rotation = Quaternion.Euler(0, 0, -270);
                break;

            // Curve pipes with random rotation
            case 'L':
                prefab = pipeCurvePrefab;
                rotation = Random90Rotation();
                break;
            case 'R':
                prefab = pipeCurveMirroredPrefab;
                rotation = Random90Rotation();
                break;

            // Start / End
            case 's':
                prefab = pipeStart;
                break;
            case 'e':
                prefab = pipeEnd;
                break;
            case 'E':
                prefab = pipeBadEnd;
                break;

            default:
                Debug.LogWarning($"[PipeManager] Unknown layout char '{c}' at {cellPos}. Skipping.");
                return;
        }

        if (!prefab)
        {
            Debug.LogError($"[PipeManager] Prefab for '{c}' is not assigned in Inspector.");
            return;
        }

        Vector3 worldPos = grid.CellToWorld(cellPos) + grid.cellSize * 0.5f;

        GameObject go = Instantiate(prefab, worldPos, rotation, transform);

        // Optional: register PipeNode if present
        PipeNode node = go.GetComponent<PipeNode>();
        if (node != null)
        {
            var key = new Vector2Int(cellPos.x, cellPos.y);
            node.nodePosition = key;

            // Avoid duplicate key crash
            if (!pipes.TryAdd(key, node))
                Debug.LogWarning($"[PipeManager] Duplicate pipe at {key} (layout overlap?).");
        }
    }

    private static Quaternion Random90Rotation()
    {
        int steps = UnityEngine.Random.Range(0, 4); // 0,1,2,3
        return Quaternion.Euler(0, 0, steps * 90);
    }

    private IEnumerator Timer()
    {
        float curTime = 0f;

        while (curTime < maxTimer)
        {
            curTime += Time.deltaTime;              // smoother for UI
            float widthPercent = curTime / maxTimer; // 0 -> 1
            float width = widthPercent * maxWidth;

            var s = timerBar.transform.localScale;
            timerBar.transform.localScale = new Vector3(width, s.y, s.z);

            yield return null; // wait 1 frame
        }

        Close();
    }

    private void Close()
    {
        Destroy(this.gameObject);
    }
    

    private void ClearChildren()
    {
        // Immediate in edit mode, Destroy in play mode
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(child);
            else Destroy(child);
#else
            Destroy(child);
#endif
        }
    }
}
