using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HoverdMouseDisplayInfo : MonoBehaviour
{

    public GameObject tooltipPanel; // Панель для підказки
    public TMP_Text itemNameText; // Текст для назви предмета
    public TMP_Text strengthText; // Текст для сили
    public TMP_Text agilityText; // Текст для спритності
    public TMP_Text intelligenceText; // Текст для інтелекту
    public TMP_Text Armour;
    public TMP_Text Qality;
    public TMP_Text Fire_RES;
    public TMP_Text Coll_RES;
    public TMP_Text Light_RES;
  //  public TMP_Text Darkness_RES;
    private void Start()
    {
        // Спочатку приховуємо панель підказки
        tooltipPanel.SetActive(false);
    }

    // Метод для відображення підказки з інформацією про предмет
    public void ShowTooltip(itemSlot slot)
    {
        if (slot == null || slot.GetDisplayName() == "Food")
        {
            tooltipPanel.SetActive(false);
            return;
        }

        tooltipPanel.SetActive(true);
        itemNameText.text = slot.GetDisplayName();

        // ПЕРЕВІРКА: Якщо це валюта
        if (slot.GetDisplayItemType() == TypeItem.Currency)
        {
            // Ховаємо всі цифри характеристик
            strengthText.gameObject.SetActive(false);
            agilityText.gameObject.SetActive(false);
            intelligenceText.gameObject.SetActive(false);
            Armour.transform.parent.gameObject.SetActive(false); // Ховаємо напис броні
            Qality.transform.parent.gameObject.SetActive(false); // Ховаємо якість
            Fire_RES.gameObject.SetActive(false);
            Coll_RES.gameObject.SetActive(false);
            Light_RES.gameObject.SetActive(false);

            // Тимчасово виведемо опис у полі назви або створи окреме текстове поле для опису:
            // Наприклад, допишемо опис під назвою:
            itemNameText.text = slot.GetDisplayName() + "\n\n<color=yellow>" + slot.GetDisplayDescription() + "</color>";
        }
        else // Якщо це звичайна екіпіровка (Броня, шолом тощо)
        {
            // Вмикаємо тексти
            strengthText.gameObject.SetActive(true);
            agilityText.gameObject.SetActive(true);
            intelligenceText.gameObject.SetActive(true);
            Armour.transform.parent.gameObject.SetActive(true);
            Qality.transform.parent.gameObject.SetActive(true);
            Fire_RES.gameObject.SetActive(true);
            Coll_RES.gameObject.SetActive(true);
            Light_RES.gameObject.SetActive(true);

            ItemAttributes displayAttributes = slot.GetDisplayAttributes();
            if (displayAttributes != null)
            {
                // ПРЕФІКСИ (Можна виділити, наприклад, блакитним або додати слово Prefix)
                strengthText.text = "<color=#66b3ff>[P]</color> + " + displayAttributes.prefixes.strength + " To Strength";
                agilityText.text = "<color=#66b3ff>[P]</color> + " + displayAttributes.prefixes.agility + " To Agility";
                intelligenceText.text = "<color=#66b3ff>[P]</color> + " + displayAttributes.prefixes.intelligence + " To Intelligence";
                Armour.text = "" + displayAttributes.prefixes.armour; // Броня теж префікс
                Qality.text = "" + displayAttributes.quality;

                // СУФІКСИ (Виділимо іншим префіксом [S])
                Fire_RES.text = "<color=#ff9999>[S]</color> + " + displayAttributes.suffixes.fireresist + "% To Fire Resistance";
                Coll_RES.text = "<color=#ff9999>[S]</color> + " + displayAttributes.suffixes.coolresist + "% To Cold Resistance";
                Light_RES.text = "<color=#ff9999>[S]</color> + " + displayAttributes.suffixes.lightinresist + "% To Lightning Resistance";
            }
        }
    }
    // Метод для приховування підказки
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        // Позиціонуємо підказку біля курсора
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition.x += 140;  // Додаємо відступ 100 пікселів по осі X
            tooltipPanel.transform.position = mousePosition;
        }
    }
}