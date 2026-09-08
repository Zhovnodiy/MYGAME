using UnityEngine;
using UnityEngine.UI;

public class ZoomAndPan : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 5f;
    [SerializeField] private RectTransform content; // Ваш Content об'єкт

    [SerializeField] private Canvas canvas;
    private ScrollRect scrollRect;
    private Vector2 lastMousePosition;
    
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        canvas = GetComponentInParent<Canvas>(); // Отримати Canvas з батьківського об’єкта

        if (canvas == null)
            Debug.LogError("Canvas не знайдено на батьківських об’єктах!");
    }

    private void Update()
    {
        // Масштабування колесом миші
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Zoom(scroll);
        }

        // Скрол за допомогою перетягування (працює автоматично через Scroll Rect)
    }

    private void Zoom(float delta)
    {
        Vector2 mouseLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            scrollRect.content,
            Input.mousePosition,
            canvas.worldCamera, // Використовуємо знайдений Canvas
            out mouseLocalPos
        );

        // Зміна масштабу
        float newScale = Mathf.Clamp(
            scrollRect.content.localScale.x + delta * zoomSpeed,
            minZoom,
            maxZoom
        );

        // Коригування позиції контенту
        Vector2 pivotPos = mouseLocalPos / scrollRect.content.rect.size;
        Vector2 contentPivot = scrollRect.content.pivot;
        scrollRect.content.pivot = pivotPos;
        scrollRect.content.localScale = Vector3.one * newScale;
        scrollRect.content.pivot = contentPivot;
    }
}