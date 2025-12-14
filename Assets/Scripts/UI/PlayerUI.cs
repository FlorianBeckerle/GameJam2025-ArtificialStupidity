using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour, IScreen
{
    
    private GameObject playerUI;
    
    private PlayerStats playerStats;


    [Header("Stats")]
    [SerializeField] 
    private float health;
    [SerializeField]
    private float mana;

    [SerializeField] 
    private float maxHealth;
    [SerializeField]
    private float maxMana;
    
    //TODO: show 3 Currencies, idk what they were
    //Currencies:
    [SerializeField] 
    private float currency;
    [SerializeField] 
    private float currency2;
    [SerializeField] 
    private float currency3;


    [Header("UI Elements")] 
    [SerializeField]
    public Image[] healthBar;
    [SerializeField]
    public Image manaBar;
    
    [SerializeField]
    public Text t_currency1;
    [SerializeField]
    public Text t_currency2;
    [SerializeField]
    public Text t_currency3;
    
    
    
    public void Enter()
    {
        //TODO: Load Player Data from PlayerStats Script and show in UI
        Debug.LogWarning($"PlayerUI Enter: not configured yet");
    }

    public void Exit()
    {
        //TODO: nothing really to do here yet
        throw new System.NotImplementedException();
    }

    /*
     * Shows/Hides PlayerUI depending on {isHidden}
     * Used when Text/Dialoge is shown to hide the other stuff
     * Uses a Fade-In/-Out effect
     */
    public void HideUI(bool isHidden)
    {
        //TODO: implement Fade-In/-Out instead of just .SetActive
        playerUI.SetActive(isHidden);
    }
    
    // ----------- Initial Set ------------ // loaded from Playerdata onLoad
    
    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        healthBar[0].fillAmount = health / maxHealth;
    }

    public void SetMaxMana(float value)
    {
        maxMana = value;
        manaBar.fillAmount = mana / maxMana;
    }

    public void SetCurrency(float value)
    {
        currency = value;
        currency2 = currency;
    }

    public void SetHealth(float value)
    {
        health = value;
        healthBar[0].fillAmount = health / maxHealth;
    }

    public void SetMana(float value)
    {
        mana = value;
        manaBar.fillAmount = mana / maxMana;
    }
    
    
    // ----------- Add  /   Substract ------------
    public void UpdateMana(float mana)
    {
        //TODO: shows current mana amount, add/substract from it depending on usecase
        this.mana += mana;
        manaBar.fillAmount = this.mana / maxMana;
    }

    public void UpdateHealth(float health)
    {
        //TODO: shows current amount of health using images
        this.health += health;
        healthBar[0].fillAmount = this.health / maxHealth;
    }

    /*
     * Update the shown Currency 1 
     * expenses < 0    gain >= 0
     */
    public void UpdateCurrency(float currency)
    {
        this.currency += currency;
    }


    public void UpdateHearts()
    {
        float healthPerHeart = 2;
        
        float amountOfHearts = Mathf.RoundToInt(health / healthPerHeart);

        for (int i = 0; i < amountOfHearts; i++)
        {
            //TODO: set Images of Hearts accordingly
        }
    }
    
}
