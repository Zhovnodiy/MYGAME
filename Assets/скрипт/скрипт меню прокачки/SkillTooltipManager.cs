using UnityEngine;
using TMPro;

public class SkillTooltip : MonoBehaviour
{
    // —инглтон, щоб легко викликати в≥кно з будь-€кого нода
    public static SkillTooltip Instance;

    [SerializeField] private GameObject tooltipWindow; // —ама панель в≥кна
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private Vector2 offset = new Vector2(1415f, 145f); // «сув в≥д курсора

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideTooltip(); // —початку ховаЇмо в≥кно
    }

    void Update()
    {
        // якщо в≥кно активне Ч воно рухаЇтьс€ за мишкою
        if (tooltipWindow.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            tooltipWindow.transform.position = mousePos + offset;
        }
    }

    public void ShowTooltip(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        tooltipWindow.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipWindow.SetActive(false);
    }
}