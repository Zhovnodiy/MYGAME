using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public TMP_Text strengthText;
    public TMP_Text agilityText;
    public TMP_Text intelligenceText;
    public TMP_Text ArmourText;
    public PlayerStats playerStats;

    public void RefreshUI()
    {
        strengthText.text = " " + playerStats.strength;
        agilityText.text = " " + playerStats.agility;
        intelligenceText.text = " " + playerStats.intelligence;
        ArmourText.text = " " + playerStats.armour;
    }

    public void UpdateStats(ItemAttributes attributes, bool isEquipping)
    {
     

        if (isEquipping)
            playerStats.UpdateStats(attributes);
        else
            playerStats.RemoveStats(attributes);

        RefreshUI();

    }

}