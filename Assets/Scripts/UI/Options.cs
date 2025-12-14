using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour, IScreen
{
    [Header("System")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Option Fields")]
    [Header("Sounds")]
    [SerializeField]
    private Slider masterVolume;
    [SerializeField]
    private Slider musicVolume;
    [SerializeField]
    private Slider vfxVolume;
    
    [Header("UI Elements")]
    [SerializeField]
    Button backButton;
    
    
    private AudioMixerGroup masterGroup;
    private AudioMixerGroup musicGroup;
    private AudioMixerGroup vfxGroup;

    [Header("Dev")] 
    //The Screen from which the Options Menu was started from
    [SerializeField] private IScreen previousCanvas;
    
    public void Enter(IScreen previousCanvas)
    {
        this.previousCanvas = previousCanvas;
        this.gameObject.SetActive(true);
        
        SetSliderValues();
    }

    private void SetSliderValues()
    {
        masterVolume.value = PlayerPrefs.GetFloat("Master");
        musicVolume.value = PlayerPrefs.GetFloat("Music");
        vfxVolume.value = PlayerPrefs.GetFloat("VFX");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioMixer == null) return;
        masterGroup = audioMixer.FindMatchingGroups("Master")[0];
        musicGroup = audioMixer.FindMatchingGroups("Music")[0];
        vfxGroup = audioMixer.FindMatchingGroups("VFX")[0];


        masterVolume.onValueChanged.AddListener(delegate { OnSaveSelection(); });
        musicVolume.onValueChanged.AddListener(delegate { OnSaveSelection(); });
        vfxVolume.onValueChanged.AddListener(delegate { OnSaveSelection(); });
        
        backButton.onClick.AddListener(delegate { OnClose(); });
    }
    
    

    // Update is called once per frame
    void OnSaveSelection()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Master", masterVolume.value);
            audioMixer.SetFloat("Music", musicVolume.value);
            audioMixer.SetFloat("VFX", vfxVolume.value);
            
        }
        
        PlayerPrefs.SetFloat("Master", masterVolume.value);
        PlayerPrefs.SetFloat("Music", musicVolume.value);
        PlayerPrefs.SetFloat("VFX", vfxVolume.value);
    }

    void OnClose()
    {
        this.previousCanvas.Enter();
        this.previousCanvas = null;
        this.gameObject.SetActive(false);
    }

    public void Enter()
    {
        Enter(this);
    }

    public void Exit()
    {
        previousCanvas?.Enter();
        this.gameObject.SetActive(false);
    }
}
