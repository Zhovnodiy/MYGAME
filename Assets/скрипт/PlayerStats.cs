using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int strength;
    public int agility;
    public int intelligence;
    public int armour;

    public TMP_Text strengthText;
    public TMP_Text agilityText;
    public TMP_Text intelligenceText;
    public TMP_Text ArmourText;
    private PlayerStatsUI playerStatsUI;

    private void Start()
    {
        playerStatsUI = FindObjectOfType<PlayerStatsUI>();

        RefreshUI();
    }

    public void UpdateStats(ItemAttributes attributes)
    {
        strength += attributes.strength;
        agility += attributes.agility;
        intelligence += attributes.intelligence;
        armour += attributes.armour;
    }

    public void RemoveStats(ItemAttributes attributes)
    {
        strength -= attributes.strength;
        agility -= attributes.agility;
        intelligence -= attributes.intelligence;
        armour -= attributes.armour;
    }

    public void AddSkillBonus(SkillData skill)
    {
        switch (skill.statType)
        {
            case SkillStatType.Strength:
                strength += skill.value;
                break;

            case SkillStatType.Dexterity:
                agility += skill.value;
                break;

            case SkillStatType.Intelligence:
                intelligence += skill.value;
                break;
        }

        if (playerStatsUI != null)
            playerStatsUI.RefreshUI();
    }


    public void RefreshUI()
    {
        if (strengthText != null)
            strengthText.text = strength.ToString();

        if (agilityText != null)
            agilityText.text = agility.ToString();

        if (intelligenceText != null)
            intelligenceText.text = intelligence.ToString();

        if (ArmourText != null)
            ArmourText.text = armour.ToString();
    }
}