using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{

    [SerializeField] private int points = 1;
    [SerializeField] private bool once = true;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used && once) return;
        if (!other.CompareTag("Interactible")) return;

        ScoreManager.Instance.Add(points);
        used = true;
    }
}
