using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string prefix = "Score; ";

    public int Score { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        UpdateUI();
    }

    public void Add(int amount = 1)
    {
        Score += amount;
        UpdateUI();
    }

    public void Subtract(int amount = 1)
    {
        Score -= amount;
        if (Score < 0) Score = 0;
        UpdateUI();
    }

    public void SetScore(int value)
    {
        Score = value;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = prefix + Score;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)){
            Add(1);
        }

        if (Input.GetKeyDown(KeyCode.L)){
            Subtract(1);
        }
    }
#endif

}
