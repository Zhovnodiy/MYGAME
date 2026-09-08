using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    public RectTransform pauseMenuUI; // Меню паузи
    public RectTransform questsMenuUI; // Меню квестів
    public Text QuestName; // Меню паузи
    public Text questTitle; // Меню квестів
    public Text QuestDescription; //ціль
    private bool isPaused = false;
    private bool isQuestsMenuOpen = false; // Перевірка, чи відкрите меню квестів
    public float animationDuration = 0.3f; // Тривалість анімації
    public float slideBackDuration = 0.15f; // Тривалість анімації руху на 30 пікселів
    private Vector3 offScreenPosition;
    private Vector3 onScreenPosition;
    private Vector3 questsOffScreenPosition;
    private Vector3 questsOnScreenPosition;
    private Vector3 questsLeftPosition; // Позиція при зміщенні на 30 пікселів вліво
    public QuestManager questManager;

    void Start()
    {
        // Початкова позиція меню паузи ліворуч за екраном
        offScreenPosition = new Vector3(-Screen.width, -7, 0);
        // Кінцева позиція меню паузи
        onScreenPosition = new Vector3(285.8889f, -7, 0);
        pauseMenuUI.anchoredPosition = offScreenPosition;

        // Початкова позиція меню квестів
        questsOffScreenPosition = new Vector3(-Screen.width - 200, -6, 0); // Початково за екраном
        // Кінцева позиція меню квестів
        questsOnScreenPosition = new Vector3(-91f, -112, 0);
        // Позиція з відступом вліво на 30 пікселів
        questsLeftPosition = new Vector3(questsOnScreenPosition.x - 30f, -6, 0);

        questsMenuUI.anchoredPosition = questsOffScreenPosition;
        Resume();
        UpdateQuestUI("Світло Підземель", "Темні коридори старовинного підземелля знову стали небезпечними. Колись це місце було джерелом потужної магії, але тепер кристали, що підтримували порядок, втратили свою силу. Твоя мета — розчистити підземелля від злих створінь, що оселилися там, і відновити живлення магічних кристалів, схованих у його глибині. Лише повернувши кристалам силу, ти зможеш запалити світло в цих похмурих залах і врятувати світ від неминучої загрози.");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        StartCoroutine(SlideOut(pauseMenuUI, offScreenPosition)); // Запускаємо анімацію для меню паузи
        if (isQuestsMenuOpen)
        {
            StartCoroutine(SlideOut(questsMenuUI, questsOffScreenPosition)); // Закриваємо меню квестів, якщо воно відкрите
        }
        Time.timeScale = 1f;
        isPaused = false;
        isQuestsMenuOpen = false; // Закриваємо меню квестів
    }

    void Pause()
    {
        StartCoroutine(SlideIn(pauseMenuUI, onScreenPosition)); // Запускаємо анімацію появи меню паузи
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowQuestsMenu()
    {
        if (!isQuestsMenuOpen)
        {
            StartCoroutine(SlideInQuests()); // Показуємо меню квестів з ефектом руху
            isQuestsMenuOpen = true;
        }
        else
        {
            StartCoroutine(SlideOut(questsMenuUI, questsOffScreenPosition)); // Ховаємо меню квестів
            isQuestsMenuOpen = false;
        }
    }
    public void UpdateQuestUI(string questName, string questDescription)
    {
        QuestName.text = questName; // Оновлюємо текст назви квесту
        questTitle.text = questDescription; // Оновлюємо текст опису квесту
        
    }
    public void CompleteQuestButton()
    {
        questManager.CompleteCurrentQuest();
    }
    // Анімація для появи меню квестів та його руху
    IEnumerator SlideInQuests()
    {
        // Спочатку показуємо меню квестів
        yield return StartCoroutine(SlideIn(questsMenuUI, questsOnScreenPosition));
        // Потім рухаємо його вліво на 30 пікселів
        yield return StartCoroutine(SlideIn(questsMenuUI, questsLeftPosition, slideBackDuration));
        // І повертаємо назад на початкову позицію
        yield return StartCoroutine(SlideIn(questsMenuUI, questsOnScreenPosition, slideBackDuration));
    }

    // Анімація для плавного появи меню
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

    // Анімація для плавного приховування меню
    IEnumerator SlideOut(RectTransform uiElement, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = uiElement.anchoredPosition;
        while (elapsedTime < animationDuration)
        {
            uiElement.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / animationDuration));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        uiElement.anchoredPosition = targetPosition;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}