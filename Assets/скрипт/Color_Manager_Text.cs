using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(TMP_Text))]
public class Color_Manager_Text : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    private Text text;

    [Header("Text Colors")]
    public Color normalColor = Color.white;
    public Color disabledColor = Color.gray;
    public Color pressedColor = Color.green;
    public Color selectedColor = Color.blue;
    public Color hoverColor = Color.red;

    private Color currentColor;

    void Start()
    {
        text = GetComponent<Text>();
        if (text != null)
        {
            currentColor = normalColor;
            text.color = currentColor;
        }
    }

    public void SetTextColor(Color color)
    {
        if (text != null)
        {
            currentColor = color;
            text.color = currentColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetTextColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetTextColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetTextColor(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetTextColor(normalColor);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetTextColor(selectedColor);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetTextColor(normalColor);
    }

    public void SetDisabledColor()
    {
        SetTextColor(disabledColor);
    }

    public void SetNormalColor()
    {
        SetTextColor(normalColor);
    }
}





