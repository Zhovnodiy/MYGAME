using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public string characterClass;
    public string login;
    public string password;
}

public class ImageSwitcher : MonoBehaviour
{
    public Image[] imageComponents;
    public Sprite[] newSprites;
    private Sprite[] originalSprites;
    private int selectedIndex = -1;

    public Image image, imageNew, image1, imageNew1, image2, imageNew2;
    public TMP_Text titleText, mainContentText;
    public TMP_InputField inputField;

    private string filePath;

    private PlayerData playerData = new PlayerData(); // Об'єкт для збереження даних

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        LoadUserData();

        originalSprites = new Sprite[imageComponents.Length];
        for (int i = 0; i < imageComponents.Length; i++)
        {
            originalSprites[i] = imageComponents[i].sprite;
        }

        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputFieldChanged);
        }
        else
        {
            Debug.LogWarning("InputField не призначено.");
        }
    }

    public void SwitchImage(int index)
    {
        if (index >= 0 && index < imageComponents.Length && index < newSprites.Length)
        {
            if (imageComponents[index] != null)
            {
                if (selectedIndex == -1)
                {
                    imageComponents[index].sprite = newSprites[index];
                    selectedIndex = index;
                    PrintIndexMessage(index);
                }
                else if (selectedIndex == index)
                {
                    imageComponents[index].sprite = originalSprites[index];
                    selectedIndex = -1;
                }
                else
                {
                    Debug.LogWarning("Спочатку скасуйте вибір поточного класу.");
                }
            }
            else
            {
                Debug.LogWarning("Компонент Image не призначено.");
            }
        }
        else
        {
            Debug.LogWarning("Недійсний індекс: " + index);
        }
    }

    private void PrintIndexMessage(int index)
    {
        switch (index)
        {
            case 0: // Воїн
                UpdateCharacterSelection("Warrior", "Warrior, the mighty class...");
                break;
            case 1: // Маг
                UpdateCharacterSelection("Mage", "Mage, the arcane master...");
                break;
            case 2: // Лучник
                UpdateCharacterSelection("Archer", "Archer, the elusive class...");
                break;
            default:
                Debug.Log("Невідомий клас.");
                break;
        }
    }

    private void UpdateCharacterSelection(string characterClass, string description)
    {
        playerData.characterClass = characterClass;
        playerData.level = 1; // Початковий рівень
        titleText.text = characterClass;
        mainContentText.text = description;
        SaveUserData();
    }

    private void OnInputFieldChanged(string newText)
    {
        playerData.playerName = newText;
        SaveUserData();
    }

    private void SaveUserData()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Дані збережено: " + jsonData);
    }

    private void LoadUserData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            Debug.Log("Дані завантажено: " + jsonData);
            titleText.text = playerData.characterClass;
            inputField.text = playerData.playerName;
        }
        else
        {
            Debug.Log("Файл з даними не знайдено. Створюємо нового персонажа.");
            playerData.playerName = "New Player";
            playerData.level = 1;
            playerData.characterClass = "None";
            playerData.login = "default_user";
            playerData.password = "password123";
            SaveUserData();
        }
    }
}




