using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IScreen
{
    [Header("UI Elements")]
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button exitButton;


    [Header("UI Canvas")]
    [SerializeField]
    private GameObject mainMenuScreen;
    

    //Get Scripts
    void Start()
    {
        startButton.onClick.AddListener(delegate { OnStartGame(); });
        exitButton.onClick.AddListener(delegate { Exit(); });
    }

    //Pressed Start Game
    void OnStartGame()
    {
        //TODO: Toggle Save Select screen
        mainMenuScreen.SetActive(false);
        
        Debug.Log("Start Game Pressed");
    }

    //Go Back to Main Menu & reset all shown screens
    public void Enter()
    {
        mainMenuScreen.SetActive(true);
        
    }

    //Pressed exit Button
    public void Exit()
    {
        Application.Quit();
    }

}
