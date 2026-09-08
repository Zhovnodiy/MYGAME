using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings : MonoBehaviour
{
    public GameObject TabGamePlay;
    public GameObject TabGrafic;
    public int IdexSelectsTab = 0;
    [SerializeField] public TMP_Dropdown[] dropdowns;
   public string selectedOption;
    private void Start()
    {
        // Ініціалізація вкладок
        TabGrafic.SetActive(false);
        TabGamePlay.SetActive(true);

        // Пошук Dropdown елементів на сцені
        dropdowns = FindObjectsOfType<TMP_Dropdown>();
    }

    private void Update()
    {
        // Обробка клавіш для перемикання вкладок
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            IdexSelectsTab = (IdexSelectsTab + 1) % 2; // Лише два таби, циклічне перемикання
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            IdexSelectsTab = (IdexSelectsTab - 1 + 2) % 2; // Запобігання негативним значенням
        }

        SwitcTabs();
        OnFullScreenChanged();
    }

    private void SwitcTabs()
    {
        // Перемикання вкладок
        switch (IdexSelectsTab)
        {
            case 0:
                TabGrafic.SetActive(false);
                TabGamePlay.SetActive(true);
                break;
            case 1:
                TabGrafic.SetActive(true);
                TabGamePlay.SetActive(false);
                break;
        }
    }


    public void OnDropdownValueChanged(TMP_Dropdown dropdown)
    {
        // Отримання тексту вибраного елемента
        selectedOption = dropdown.options[dropdown.value].text;
        Debug.Log($"Dropdown: {dropdown.name}, Selected Option: {selectedOption}");
    }

    private void OnFullScreenChanged()
    {

        if (selectedOption == "Fullscreen")
        {

            //   Screen.fullScreen = true; // Перехід в повноекранний режим
            Debug.Log("Switching to Fullscreen");
        }
        if (selectedOption == "Windowed (Fullscreen)")
        {
            // Якщо вибрано Windowed
            //  Screen.fullScreen = false; // Перехід в віконний режим
            // Screen.SetResolution(1280, 720, false); // Встановлення розміру вікна (ширина, висота)
            Debug.Log("Switching to Windowed Mode with resolution 1280x720");
        }
        if (selectedOption == "Windowed ")
        {
            // Якщо вибрано Borderless (безрамковий віконний режим)
            // Screen.fullScreen = false; // Віконний режим
            //  Screen.SetResolution(1920, 1080, false); // Встановлення розміру вікна (ширина, висота)
            //  WindowBorderlessMode();
            //  Debug.Log("Switching to Borderless Window");
        }

           
              // Debug.Log("Invalid Selection");
                
        
    }
    private void WindowBorderlessMode()
    {
       
        Screen.SetResolution(1920, 1080, false); // Розмір вікна без рамки
        
    }
}


