using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_Check : MonoBehaviour
{
    public GameObject objectToHide; // Об'єкт, який потрібно зробити невидимим

    // Метод, який буде викликатися при натисканні на кнопку
    public void OnButtonClick()
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // Робимо об'єкт невидимим
        }
        else
        {
            Debug.LogError("Object to hide is not assigned.");
        }
    }
    public void sdsdsds()
    {

    }
}