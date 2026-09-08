using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadBar : MonoBehaviour
{
    public Image loadingBar; // Image з Fill Amount для шкали завантаження
    public RectTransform pilot; // RectTransform пілота або іншого зображення, яке потрібно рухати
    public TMP_Text percentageText; // Текст для відображення відсотка заповнення
    public string sceneToLoad = "GameScene"; // Назва сцени для завантаження

    public float secondsPer10Percent = 0.1f; // Час для 10 відсотків (в секундах)

    private void Start()
    {
        StartCoroutine(LoadProgress());
    }

    private IEnumerator LoadProgress()
    {
        // Починаємо асинхронне завантаження сцени
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        // Забороняємо автоматичну активацію сцени до повного завантаження
        operation.allowSceneActivation = false;

        // Прогрес завантаження (від 0 до 1)
        float progress = 0f;

        // Час, необхідний для досягнення 100%
        float timeToLoad = secondsPer10Percent * 10f; // 10% заповнюється за secondsPer10Percent, тобто 10% = 1 секунда

        // Час для кожного оновлення
        float timePerUpdate = timeToLoad / 10f; // Час для кожного оновлення

        // Процес завантаження
        for (int i = 0; i <= 100; i++)
        {
            progress = i / 100f; // Обчислюємо прогрес
            loadingBar.fillAmount = progress; // Оновлюємо шкалу
            percentageText.text = i + "%"; // Оновлюємо відсотки

            MovePilot(progress); // Оновлюємо позицію пілота

            // Якщо прогрес досяг 100%, активуємо сцену
            if (i == 100)
            {
                operation.allowSceneActivation = true;
            }

            yield return new WaitForSeconds(timePerUpdate); // Чекаємо перед наступним оновленням
        }
    }

    private void MovePilot(float progress)
    {
        // Визначаємо нову позицію пілота на основі прогресу
        float targetPositionX = loadingBar.rectTransform.sizeDelta.x * progress; // Множимо на прогрес для руху
        pilot.anchoredPosition = new Vector2(targetPositionX, pilot.anchoredPosition.y); // Оновлюємо позицію пілота
    }
}
