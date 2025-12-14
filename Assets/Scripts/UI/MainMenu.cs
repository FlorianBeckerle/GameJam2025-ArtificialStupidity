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
    private Button optionsButton;
    [SerializeField]
    private Button exitButton;


    [Header("UI Canvas")]
    [SerializeField]
    private GameObject mainMenuScreen;
    [SerializeField]
    private GameObject saveSelectionScreen;
    [SerializeField]
    private GameObject optionsScreen;

    [Header("Scripts")]
    [SerializeField]
    private MenuSaveSlot saveSelection;
    [SerializeField]
    private Options options;

    //Get Scripts
    void Start()
    {
        saveSelection = saveSelectionScreen.GetComponent<MenuSaveSlot>();
        options = optionsScreen.GetComponent<Options>();

        startButton.onClick.AddListener(delegate { OnStartGame(); });
        optionsButton.onClick.AddListener(delegate { OnOptionsMenu(); });
        exitButton.onClick.AddListener(delegate { Exit(); });


    }

    //Pressed Start Game
    void OnStartGame()
    {
        //TODO: Toggle Save Select screen
        mainMenuScreen.SetActive(false);
        saveSelectionScreen.SetActive(true);
        Debug.Log("Start Game Pressed");
    }

    //Go Back to Main Menu & reset all shown screens
    public void Enter()
    {
        mainMenuScreen.SetActive(true);
        saveSelectionScreen.SetActive(false);
        optionsScreen.SetActive(false);
    }

    //Pressed Options Screen
    void OnOptionsMenu()
    {
        //TODO: Toggle Options Overlay
        mainMenuScreen.SetActive(false);
        options.Enter(this);
    }

    //Pressed exit Button
    public void Exit()
    {
        Application.Quit();
    }

}
