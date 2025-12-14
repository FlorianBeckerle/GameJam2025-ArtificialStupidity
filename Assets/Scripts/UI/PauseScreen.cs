using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour, IScreen
{
    
    [Header("UI Elements")]
    [SerializeField]
    private Button resumeButton; //Close Pause Menu
    [SerializeField]
    private Button optionButton; //Open Settings
    [SerializeField]
    private Button mainMenuButton; //Quit to Main Menu
    [SerializeField] 
    private Button exitButton; //Quit to Desktop
    
    [SerializeField]
    private GameObject pauseScreen;
    
    [SerializeField] 
    private Options options;
    
    [SerializeField] private InputRouter router;

    private bool isPaused = false;


    void Start()
    {
        resumeButton.onClick.AddListener( delegate { Exit(); } );
        mainMenuButton.onClick.AddListener( delegate { OnMainMenu(); } );
        optionButton.onClick.AddListener( delegate { OnOptions(); } );
        if (router != null)
        {
            router.PausePressed += delegate { HandlePauseInput(); };    
        }
        else
        {
            Debug.LogError("[PauseScreen] Router is null.");
        }
        
    }
    
    public void Enter()
    {
        isPaused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    //
    public void Exit()
    {
        isPaused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnOptions()
    {
        if(options != null)
            options.Enter(this);
        pauseScreen.SetActive(false);
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1;
        Debug.Log("[PauseScreen] OnMainMenu");
        SceneManager.LoadScene("MainMenu");
    }

    public void HandlePauseInput()
    {
        //Switch input
        isPaused = !isPaused;

        if (isPaused)
        {
            Enter(); //Open UI
        }
        else
        {
            Exit(); //Close UI
        }
    }
}
