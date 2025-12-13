using UnityEngine;
using TMPro;
using System.Collections;
public class TextSystem : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject textBox;
    [SerializeField] private TMP_Text textLabel;

    private bool isShowing;

    void Awake()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Show("This is a test message. Press F or Escape to close.");
        }

        if (isShowing && Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
        
    }

    public void Show(string message)
    {
        textBox.SetActive(true);
        textLabel.text = message;
        isShowing = true;
    }

    public void Hide()
    {
        textBox.SetActive(false);
        isShowing = false;
    }
}
