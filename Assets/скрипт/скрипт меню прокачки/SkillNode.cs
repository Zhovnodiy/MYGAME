using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Обов'язково для роботи з кольором та Image
using System.Collections.Generic;

// Додаємо інтерфейс IPointerClickHandler, щоб Unity бачила кліки
public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{


    [Header("ID & Visuals")]
    public string nodeID;
    public Image nodeImage;

    [Header("Skill")]
    public SkillData skillData;

    [Header("Connections")]
    public List<SkillNode> neighbours = new List<SkillNode>();

    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Тьмяний сірий
    public Color unlockedColor = Color.white; // Чистий білий (RGB 255, 255, 255)



    [Header("State")]
    public bool isUnlocked = false;
    public bool isStartNode = false; // Постав галочку ТІЛЬКИ на одному ноді, з якого починається гра!


    void Start()
    {

        if (nodeImage == null)
            nodeImage = GetComponent<Image>();

        if (skillData != null && skillData.icon != null)
            nodeImage.sprite = skillData.icon;

        if (isStartNode)
            isUnlocked = true;

        UpdateVisual();
    }

    // Метод перефарбовування нода
    public void UpdateVisual()
    {
        if (nodeImage != null)
        {
            nodeImage.color = isUnlocked ? unlockedColor : lockedColor;
        }
    }

    // НОВИЙ МЕТОД: Спрацьовує при кліку по ноду
    public void OnPointerClick(PointerEventData eventData)
    {

        SkillTreeManager.Instance.TryUnlock(this);
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
        SaveNode();
    }

    public void SaveNode()
    {
        PlayerPrefs.SetInt("SkillNode_" + nodeID, 1);
        PlayerPrefs.Save();
    }
    public void LoadNode()
    {
        if (PlayerPrefs.GetInt("SkillNode_" + nodeID, 0) == 1)
        {
            isUnlocked = true;
            UpdateVisual();
        }
    }
    public void UnlockFromSave()
    {
        isUnlocked = true;

        UpdateVisual();

        PlayerStats player = FindObjectOfType<PlayerStats>();

        if (player != null)
        {
            player.AddSkillBonus(skillData);
        }
    }

    // --- ТВОЯ ЛОГІКА ТУЛТІПІВ (БЕЗ ЗМІН) ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillData == null)
            return;

        SkillTooltip.Instance.ShowTooltip(
            skillData.skillName,
            skillData.description
        );
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        SkillTooltip.Instance.HideTooltip();
    }

    void OnDisable()
    {
        if (SkillTooltip.Instance != null)
        {
            SkillTooltip.Instance.HideTooltip();
        }
    }
}