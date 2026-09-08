using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Color_Manager_Image : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{

    private Image image;

    [Header("Image Colors")]
    public Color normalColor = Color.white;
    public Color disabledColor = Color.gray;
    public Color pressedColor = Color.green;
    public Color selectedColor = Color.blue;
    public Color hoverColor = Color.red;

    private Color currentColor;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            currentColor = normalColor;
            image.color = currentColor;
        }
    }

    public void SetImageColor(Color color)
    {
        if (image != null)
        {
            currentColor = color;
            image.color = currentColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetImageColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetImageColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetImageColor(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetImageColor(normalColor);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetImageColor(selectedColor);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetImageColor(normalColor);
    }

    public void SetDisabledColor()
    {
        SetImageColor(disabledColor);
    }

    public void SetNormalColor()
    {
        SetImageColor(normalColor);
    }
}
