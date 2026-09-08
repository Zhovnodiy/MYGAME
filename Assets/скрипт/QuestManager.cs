using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>(); // Список квестів
    public Text questNameText; // Текстовий компонент для назви квесту
    public Text questDescriptionText; // Текстовий компонент для опису квесту
    public Text goalDescriptionText;
    private int currentQuestIndex = 0; // Індекс поточного квесту

    void Start()
    {
        // Додаємо квести
        quests.Add(new Quest("Світло Підземель", "Темні коридори старовинного підземелля знову стали небезпечними. Колись це місце було джерелом потужної магії, але тепер кристали, що підтримували порядок, втратили свою силу. Твоя мета — розчистити підземелля від злих створінь, що оселилися там, і відновити живлення магічних кристалів, схованих у його глибині. Лише повернувши кристалам силу, ти зможеш запалити світло в цих похмурих залах і врятувати світ від неминучої загрози.","Дістаньтесь до підземелля"));
        quests.Add(new Quest("Забутий Охоронець", "У найглибших закутках підземелля, де тіні танцюють на стінах, ти натрапляєш на древню кам'яницю, що охороняє магічний кристал. Цей кристал, колись джерело світла і сили, тепер захищає забутий охоронець — Грімост.Грімост був створений мудрецями давніх часів, щоб оберігати кристал від темряви. Але після загибелі магів, які його втілили, він втратив свій сенс і піддався впливу темних сил. Тепер, спотворений злом, він стає безжальним ворогом для всіх, хто намагається наблизитися до кристала.", "Подолати Босса "));
        quests.Add(new Quest("Переможи ворога", "Натисни на кнопку У, щоб перемогти ворога.","fdfdfdfdf"));

        // Оновлюємо UI для першого квесту
        UpdateQuestUI();

        // Призначаємо дії після завершення кожного квесту
        quests[0].onComplete = () => { Debug.Log("Квест 1 завершено. Відкриваємо наступний!"); NextQuest(); };
        quests[1].onComplete = () => { Debug.Log("Квест 2 завершено. Відкриваємо наступний!"); NextQuest(); };
        quests[2].onComplete = () => { Debug.Log("Квест 3 завершено. Всі квести завершені!"); };
    }

    void Update()
    {
        // Перевіряємо натискання клавіш для кожного квесту
        if (currentQuestIndex == 0 && Input.GetKeyDown(KeyCode.S)) // Натискання Ш
        {
            CompleteCurrentQuest();
        }
        else if (currentQuestIndex == 1 && Input.GetKeyDown(KeyCode.K)) // Натискання К
        {
            CompleteCurrentQuest();
        }
        else if (currentQuestIndex == 2 && Input.GetKeyDown(KeyCode.U)) // Натискання У
        {
            CompleteCurrentQuest();
        }
    }

    // Оновлення UI для поточного квесту
    void UpdateQuestUI()
    {
        if (currentQuestIndex < quests.Count)
        {
            questNameText.text = quests[currentQuestIndex].questName;
            questDescriptionText.text = quests[currentQuestIndex].description;
            goalDescriptionText.text = quests[currentQuestIndex].goalDescription; // Оновлюємо текст цілі квесту
        }
        else
        {
            questNameText.text = "Немає квестів";
            questDescriptionText.text = "";
            goalDescriptionText.text = ""; // Очищаємо текст цілі, якщо немає квестів
        }
    }

    // Виконати поточний квест
    public void CompleteCurrentQuest()
    {
        if (currentQuestIndex < quests.Count)
        {
            quests[currentQuestIndex].CompleteQuest();
        }
    }

    // Перехід до наступного квесту
    void NextQuest()
    {
        currentQuestIndex++;
        UpdateQuestUI();
    }
}
