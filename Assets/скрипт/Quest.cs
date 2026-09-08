using UnityEngine;

public class Quest
{
    public string questName; // Назва квесту
    public string description; // Опис квесту
    public bool isCompleted; // Статус виконання квесту
    public System.Action onComplete; // Дія, яка виконується після завершення
    public string goalDescription; // Ціль квесту


    // Конструктор для ініціалізації квесту
    public Quest(string name, string desc, string goal)
    {
        questName = name;
        description = desc;
        isCompleted = false;
        goalDescription = goal;

    }

    // Виконання квесту
    public void CompleteQuest()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            onComplete?.Invoke(); // Викликати дію після завершення
            Debug.Log(questName + " завершено!");
        }
    }
}
