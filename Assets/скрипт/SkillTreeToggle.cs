using UnityEngine;

public class SkillTreeToggle : MonoBehaviour
{
    // Додаємо змінну для посилання на об'єкт дерева скілів
    public GameObject skillTree;

    // Масив для зберігання текстових вікон
    public GameObject[] textWindows;

    // За замовчуванням робимо дерево вимкненим
    void Start()
    {
        skillTree.SetActive(false); // Вимикаємо дерево скілів при старті
        foreach (GameObject textWindow in textWindows)
        {
            textWindow.SetActive(false);
        }
    }

    void Update()
    {
        // Перевіряємо, чи користувач натиснув на кнопку "O"
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Змінюємо стан видимості дерева скілів
            skillTree.SetActive(!skillTree.activeSelf);
        }
    }

    // Метод для активації текстового вікна
    public void ShowTextWindow(int index)
    {
        // Деактивуємо всі текстові вікна
        foreach (GameObject textWindow in textWindows)
        {
            textWindow.SetActive(false);
        }

        // Активуємо текстове вікно з вказаним індексом
        if (index >= 0 && index < textWindows.Length)
        {
            textWindows[index].SetActive(true);
        }
    }

    // Метод для відображення текстового вікна 1
    public void InfoSlick1()
    {
        ShowTextWindow(0); // Відображаємо текстове вікно 1
    }

    // Метод для відображення текстового вікна 2
    public void InfoSlick2()
    {
        ShowTextWindow(1); // Відображаємо текстове вікно 2
    }

    // Метод для відображення текстового вікна 3
    public void InfoSlick3()
    {
        ShowTextWindow(2); // Відображаємо текстове вікно 3
    }

    // Метод для відображення текстового вікна 4
    public void InfoSlick4()
    {
        ShowTextWindow(3); // Відображаємо текстове вікно 4
    }
}
