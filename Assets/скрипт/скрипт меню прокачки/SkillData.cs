using UnityEngine;

public enum SkillStatType
{
    Strength,
    Dexterity,
    Intelligence
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill Tree/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Info")]
    public string skillName;

    [TextArea(3, 5)]
    public string description;

    public Sprite icon;

    [Header("Bonus")]
    public SkillStatType statType;

    public int value = 10;
}