using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NEXT_SCENES : MonoBehaviour
{
    // Поле для вибору номера сцени в інспекторі
    [SerializeField] private int sceneIndex;

    // Метод для завантаження сцени за індексом
    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}