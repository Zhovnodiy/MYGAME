using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(itemSlot))]
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private itemSlot _slot;
    private GameObject _dragIcon;
    private Canvas _canvas;
    private RectTransform _dragIconTransform;

    private void Awake()
    {
        _slot = GetComponent<itemSlot>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_slot.quantity == 0) return;

        _dragIcon = new GameObject("Drag Icon");
        _dragIcon.transform.SetParent(_canvas.transform, false);
        _dragIconTransform = _dragIcon.AddComponent<RectTransform>();
        _dragIconTransform.sizeDelta = new Vector2(50, 50);

        Image image = _dragIcon.AddComponent<Image>();
        image.sprite = _slot.itemsprite;
        image.raycastTarget = false;

        _slot.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
            _dragIconTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon == null) return;

        _slot.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(_dragIcon);

        GameObject targetObject = null;
        if (eventData.pointerCurrentRaycast.isValid)
        {
            targetObject = eventData.pointerCurrentRaycast.gameObject;
        }

        itemSlot targetSlot = targetObject?.GetComponent<itemSlot>();
        if (targetSlot != null && targetSlot != _slot)
        {
            SwapSlots(_slot, targetSlot);
        }
    }

    private void SwapSlots(itemSlot source, itemSlot target)
    {
        if (target.slotType == SlotType.Equipment && target.itemName != source.itemName)
        {
            Debug.Log("This item cannot be moved to that equipment slot.");
            return;
        }

        if (source.HasInstance || target.HasInstance)
        {
            SwapInstanceSlots(source, target);
            return;
        }

        SwapLegacySlots(source, target);
    }

    private static void SwapInstanceSlots(itemSlot source, itemSlot target)
    {
        bool targetIsEmpty = target.quantity == 0;
        if (!source.HasInstance || (!target.HasInstance && !targetIsEmpty))
        {
            Debug.LogWarning("Cannot mix ItemInstance data with a populated legacy slot during this migration stage.");
            return;
        }

        ItemInstance sourceInstance = source.instance;
        ItemInstance targetInstance = target.instance;

        target.SetInstance(sourceInstance);
        if (targetInstance != null)
        {
            source.SetInstance(targetInstance);
        }
        else
        {
            source.Clear();
        }

        Debug.Log($"Moved ItemInstance '{sourceInstance.templateId}' to slot {target.slotID}.");
    }

    private static void SwapLegacySlots(itemSlot source, itemSlot target)
    {
        ItemAttributes tempAttributes = target.attributes;
        Sprite tempSprite = target.itemsprite;
        string tempName = target.itemName;
        int tempQuantity = target.quantity;
        Rarity tempRarity = target.rarity;

        target.itemName = source.itemName;
        target.quantity = source.quantity;
        target.itemsprite = source.itemsprite;
        target.rarity = source.rarity;
        target.attributes = source.attributes;

        source.itemName = tempName;
        source.quantity = tempQuantity;
        source.itemsprite = tempSprite;
        source.rarity = tempRarity;
        source.attributes = tempAttributes;

        Debug.Log($"Moved {target.itemName} to slot {target.slotID}.");
    }
}