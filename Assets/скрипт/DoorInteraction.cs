using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public string doorName;  // Назва дверей, яку ви можете вказати в інспекторі
    public Color highlightColor = Color.yellow;  // Колір підсвічування
    public Color normalColor = Color.white;      // Початковий колір
    private Renderer doorRenderer;
    private bool isHovering = false;
    public Transform player; // Посилання на об'єкт героя
    public Vector3 teleportPosition; // Координати, куди телепортувати героя

    

    void Start()
    {
        doorRenderer = GetComponent<Renderer>();
        if (doorRenderer == null)
        {
            Debug.LogError("Немає Renderer на дверях");
        }
    }

    void OnMouseEnter()
    {
        // Коли наводимо мишку на двері
        isHovering = true;
        doorRenderer.material.color = highlightColor;
        Debug.Log($"Навели мишку на двері: {doorName}");
    }

    void OnMouseExit()
    {
        // Коли мишка покидає двері
        isHovering = false;
        doorRenderer.material.color = normalColor;
        Debug.Log("Мишка покинула двері");
    }

    void Update()
    {
        // Перевірка, чи наводимо на двері та натискаємо праву кнопку миші
        if (isHovering == true && Input.GetMouseButtonDown(1)) // 1 - права кнопка миші
        {
            Debug.Log($"Натиснуто праву кнопку на дверях: {doorName}. Перехід на іншу сцену.");
            // Тут ви можете додати код для переходу на іншу сцену, наприклад:
            // SceneManager.LoadScene("Назва_сцени");
            player.position = teleportPosition;
           
        }
    }
}
