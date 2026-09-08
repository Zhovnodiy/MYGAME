using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimator : MonoBehaviour
{
    public RectTransform inventoryMenuUI; // Меню інвентаря
    public RectTransform equipmentMenuUI; // Меню екіпіровки
    public RectTransform statsMenuUI; // Меню статистики
    public float animationDuration = 0.3f; // Тривалість анімації

    void Start()
    {
        // Не потрібно ініціалізувати тут shiftRightPosition, бо ми будемо змінювати зміщення безпосередньо
    }

    // Метод для обробки натискання кнопки для приховування статистики та зміщення інших меню вправо
    public void OnButtonClick()
    {
        StartCoroutine(AnimateMenus());
    }

    // Анімація для приховування статистики та зміщення інших меню вправо
    IEnumerator AnimateMenus()
    {
        // Відразу приховуємо меню статистики (просто вимикаємо його)
        statsMenuUI.gameObject.SetActive(false);

        // Зміщення інвентаря вправо на 400 пікселів
        Vector3 inventoryShiftRightPosition = new Vector3(inventoryMenuUI.anchoredPosition.x + 430, inventoryMenuUI.anchoredPosition.y, 0);
        StartCoroutine(SlideIn(inventoryMenuUI, inventoryShiftRightPosition));

        // Зміщення меню екіпіровки вправо на 200 пікселів
        Vector3 equipmentShiftRightPosition = new Vector3(equipmentMenuUI.anchoredPosition.x + 430, equipmentMenuUI.anchoredPosition.y, 0);
        StartCoroutine(SlideIn(equipmentMenuUI, equipmentShiftRightPosition));

        yield return null;
    }

    // Анімація для плавного переміщення UI елементів
    IEnumerator SlideIn(RectTransform uiElement, Vector3 targetPosition, float duration = -1f)
    {
        if (duration < 0f) duration = animationDuration; // Використовуємо основну тривалість, якщо не задана

        float elapsedTime = 0f;
        Vector3 startPosition = uiElement.anchoredPosition;
        while (elapsedTime < duration)
        {
            uiElement.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        uiElement.anchoredPosition = targetPosition;
    }
}
