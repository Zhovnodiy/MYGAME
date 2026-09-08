using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;
    [SerializeField]
    private PlayerStats playerStats;

    private LevelingSystem levelingSystem;

    [Header("Connections")]
    public SkillLine[] allLines;

    private SkillNode[] allNodes;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (allLines == null || allLines.Length == 0)
        {
            allLines = FindObjectsOfType<SkillLine>();
            Debug.Log($"Знайдено ліній: {allLines.Length}");
        }
        allNodes = FindObjectsOfType<SkillNode>();
    }

    void Start()
    {
       
        LoadTree();

        OnNodeUnlocked();

        levelingSystem = FindObjectOfType<LevelingSystem>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats не знайдено!");
        }

        if (levelingSystem == null)
        {
            Debug.LogError("Не знайдено LevelingSystem!");
        }
    }
    private void LoadTree()
    {
        foreach (SkillNode node in allNodes)
        {
            if (PlayerPrefs.GetInt("SkillNode_" + node.nodeID, 0) == 1)
            {
                node.UnlockFromSave();
            }
        }

        OnNodeUnlocked();
    }

    public bool CanUnlockNode(SkillNode node)
    {
        if (node.isStartNode)
            return true;

        Debug.Log($"Перевіряємо {node.skillData.skillName}");

        foreach (SkillNode neighbour in node.neighbours)
        {
            if (neighbour == null)
                continue;

            Debug.Log($"{neighbour.skillData.skillName}  unlocked = {neighbour.isUnlocked}");

            if (neighbour.isUnlocked)
                return true;
        }

        return false;
    }

    public bool TryUnlock(SkillNode node)
    {
        if (node == null)
            return false;

        if (node.isUnlocked)
            return false;

        if (levelingSystem.skillPoint <= 0)
        {
            Debug.Log("Недостатньо очок навичок!");
            return false;
        }

        if (!CanUnlockNode(node))
        {
            Debug.Log("Не можна відкрити цей вузол.");
            return false;
        }

        levelingSystem.skillPoint--;

        node.Unlock();
        playerStats.AddSkillBonus(node.skillData);
        Debug.Log("Strength = " + playerStats.strength);
        Debug.Log("Agility = " + playerStats.agility);
        Debug.Log("Intelligence = " + playerStats.intelligence);

        OnNodeUnlocked();

        Debug.Log("Відкрито " + node.skillData.skillName);
        Debug.Log("Очок залишилось: " + levelingSystem.skillPoint);

        return true;
    }


    public void OnNodeUnlocked()
    {
        foreach (SkillLine line in allLines)
        {
            if (line != null)
            {
                line.UpdateLineVisual();
            }
        }
    }
}