using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    
    [SerializeField]
    private PlayerData playerData;
    
    [SerializeField]
    private PlayerUI playerUI;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveSystem.CurrentGameData == null) return;
        playerData =  SaveSystem.CurrentGameData.player;
        
        if (playerData == null) return;
        InitiatePlayerUI();
    }

    /*
     * Initiate the player UI --> Access the UI and set stats
     * 
     */
    private void InitiatePlayerUI()
    {
        //Health
        playerUI.SetMaxHealth(playerData.maxHealth);
        playerUI.SetHealth(playerData.health);
        
        //Mana
        playerUI.SetMaxMana(playerData.maxMana);
        playerUI.SetMana(playerData.mana);
        
        //Currency
        //TODO: add a way to save Currencies
        playerUI.SetCurrency(1000f); //Placeholder
    }
}
