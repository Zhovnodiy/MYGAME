using UnityEngine;
using UnityEngine.UI;

public class SkillLine : MonoBehaviour
{
    [Header("Connected Nodes")]
    public SkillNode nodeA; // Перший нод зв'язку
    public SkillNode nodeB; // Другий нод зв'язку

    [Header("Visuals")]
    public Image lineImage; // Компонент Image лінії
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.6f); // Сіра неактивна лінія
    public Color unlockedColor = new Color(0f, 0.45f, 1f, 1f); // Красивий синій колір (RGB)

    void Start()
    {
        if (lineImage == null)
            lineImage = GetComponent<Image>();

        UpdateLineVisual();
    }

    // Головна логіка перевірки зв'язку
    public void UpdateLineVisual()
    {
        if (nodeA == null || nodeB == null) return;

        // ЯКЩО прокачані обидва ноди (Нод 1 ТА Нод 2) -> лінія загорається синім
        if (nodeA.isUnlocked && nodeB.isUnlocked)
        {
            lineImage.color = unlockedColor;
        }
        else
        {
            lineImage.color = lockedColor;
        }
    }
}