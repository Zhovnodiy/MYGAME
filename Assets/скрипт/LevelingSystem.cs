using UnityEngine;
using UnityEngine.UI;

public class LevelingSystem : MonoBehaviour
{
  
    public int skillPoint, AbillytiPoint = 0;
    public int currentLevel = 1;        // Поточний рівень
    public float currentExp = 0f;       // Поточний досвід
    public float expToNextLevel = 100f; // Досвід, потрібний для наступного рівня
    public float expMultiplier = 1.5f;  // Множник збільшення досвіду для кожного наступного рівня

    public Image expBar;                // Шкала досвіду (fillAmount)
    public Text levelText;              // Текст для відображення рівня

    void Start()
    {
        LoadProgress(); // Завантажуємо збережені дані про рівень і досвід
        UpdateLevelUI();
    }

    void Update()
    {
        // Перевірка на натискання клавіші "З"
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GainExperience(10000f); // Додаємо 10 одиниць досвіду при натисканні клавіші "З"
        }
    }

    public void GainExperience(float amount)
    {
        currentExp += amount;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        // Оновлюємо шкалу досвіду
        expBar.fillAmount = currentExp / expToNextLevel;
        SaveProgress(); // Зберігаємо прогрес після зміни досвіду
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp = 0f;
        expToNextLevel *= expMultiplier; // Збільшуємо кількість досвіду для наступного рівня
        UpdateLevelUI();
        SaveProgress(); // Зберігаємо прогрес після підвищення рівня
        AbillytiPoint += 1;
        skillPoint += 1;
    }

    void UpdateLevelUI()
    {
        levelText.text = "Level: " + currentLevel;
        expBar.fillAmount = currentExp / expToNextLevel; // Оновлюємо шкалу досвіду
    }

    // Збереження рівня та досвіду
    void SaveProgress()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel); // Зберігаємо рівень
        PlayerPrefs.SetFloat("currentExp", currentExp);   // Зберігаємо досвід
        PlayerPrefs.SetFloat("expToNextLevel", expToNextLevel); // Зберігаємо необхідний досвід для наступного рівня
        PlayerPrefs.Save(); // Зберігаємо всі дані
    }

    // Завантаження рівня та досвіду
    void LoadProgress()
    {
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("currentLevel"); // Завантажуємо рівень
            currentExp = PlayerPrefs.GetFloat("currentExp");   // Завантажуємо досвід
            expToNextLevel = PlayerPrefs.GetFloat("expToNextLevel"); // Завантажуємо необхідний досвід для наступного рівня
        }
    }
}
