using System.Runtime.CompilerServices;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{

    [TextArea(2,6)]
    public string message = "Default Trigger message.";

    public bool showOnce = true;

    private bool used;
    private TextSystem textSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textSystem = FindFirstObjectByType<TextSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (used && showOnce) return;
        if (!other.CompareTag("Player")) return;

        textSystem.Show(message);
        used = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
