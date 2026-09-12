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
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _slot = GetComponent<itemSlot>();
        _canvas = GetComponentInParent<Canvas>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_slot.quantity <= 0 && !ItemInstance.IsReal(_slot.instance))
            return;

        _dragIcon = new GameObject("Drag Icon");
        _dragIcon.transform.SetParent(_canvas.transform, false);

        _dragIconTransform = _dragIcon.AddComponent<RectTransform>();
        _dragIconTransform.sizeDelta = new Vector2(50, 50);

        Image image = _dragIcon.AddComponent<Image>();
        image.sprite = _slot.itemsprite;
        image.raycastTarget = false;

        CanvasGroup canvasGroup = _slot.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
            _dragIconTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
            return;

        CanvasGroup canvasGroup = _slot.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        GameObject targetObject = null;

        if (eventData.pointerCurrentRaycast.isValid)
            targetObject = eventData.pointerCurrentRaycast.gameObject;

        itemSlot targetSlot = null;

        if (targetObject != null)
            targetSlot = targetObject.GetComponentInParent<itemSlot>();

        // Видаляємо візуальну іконку перетягування
        Destroy(_dragIcon);
        _dragIcon = null;

        if (_inventoryManager == null)
            _inventoryManager = FindObjectOfType<InventoryManager>();

        // ---------------------------------------------------------
        // DROP НЕ НА СЛОТ
        // ---------------------------------------------------------

        if (targetSlot == null)
        {
            Debug.Log("Item returned: drop was outside an item slot.");
            return;
        }

        // Не можна перетягнути предмет сам на себе
        if (targetSlot == _slot)
        {
            Debug.Log("Item returned: same slot.");
            return;
        }

        // ---------------------------------------------------------
        // Перевіряємо, що targetSlot справді є одним із наших слотів
        // ---------------------------------------------------------

        bool isValidSlot = false;

        // Inventory slots
        if (_inventoryManager != null &&
            _inventoryManager.itemSlot != null)
        {
            foreach (itemSlot inventorySlot in _inventoryManager.itemSlot)
            {
                if (inventorySlot == targetSlot)
                {
                    isValidSlot = true;
                    break;
                }
            }
        }

        // Equipment slots
        if (!isValidSlot && _inventoryManager != null)
        {
            if (targetSlot == _inventoryManager.helmetSlot ||
                targetSlot == _inventoryManager.chestSlot ||
                targetSlot == _inventoryManager.legsSlot ||
                targetSlot == _inventoryManager.bootsSlot)
            {
                isValidSlot = true;
            }
        }

        // Якщо це просто рамка, фон або інший UI-об'єкт
        if (!isValidSlot)
        {
            Debug.Log("Item returned: invalid drop target.");
            return;
        }

        // ---------------------------------------------------------
        // Нормальний drop
        // ---------------------------------------------------------

        SwapSlots(_slot, targetSlot);
    }

    private void SwapSlots(itemSlot source, itemSlot target)
    {
        if (_inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found.");
            return;
        }

        // =========================================================
        // 1. INVENTORY -> INVENTORY
        // =========================================================

        if (source.slotType == SlotType.Inventory &&
            target.slotType == SlotType.Inventory)
        {
            if (source.HasInstance || target.HasInstance)
                SwapInstanceSlots(source, target);
            else
                SwapLegacySlots(source, target);

            return;
        }

        // =========================================================
        // 2. INVENTORY -> EQUIPMENT
        // =========================================================

        if (source.slotType == SlotType.Inventory &&
            target.slotType == SlotType.Equipment)
        {
            if (!CanEquipToTarget(source, target))
            {
                Debug.Log(
                    $"Cannot equip {source.GetDisplayName()} " +
                    $"into equipment slot {target.slotID}."
                );

                return;
            }

            bool equipped = _inventoryManager.EquipItem(source);

            if (!equipped)
            {
                Debug.LogWarning("EquipItem failed.");
            }

            return;
        }

        // =========================================================
        // 3. EQUIPMENT -> INVENTORY
        // =========================================================

        if (source.slotType == SlotType.Equipment &&
            target.slotType == SlotType.Inventory)
        {
            if (target.quantity > 0 || ItemInstance.IsReal(target.instance))
            {
                Debug.Log("Target inventory slot is occupied.");
                return;
            }

            _inventoryManager.UnequipItemToSlot(source, target);
            return;
        }

        // =========================================================
        // 4. EQUIPMENT -> EQUIPMENT
        // =========================================================

        if (source.slotType == SlotType.Equipment &&
            target.slotType == SlotType.Equipment)
        {
            Debug.Log("Equipment -> Equipment is not supported.");
            return;
        }
    }

    private bool CanEquipToTarget(itemSlot source, itemSlot target)
    {
        if (!ItemInstance.IsReal(source.instance))
            return false;

        TypeItem type = source.GetDisplayItemType();

        switch (type)
        {
            case TypeItem.Helmet:
                return target == _inventoryManager.helmetSlot;

            case TypeItem.Chest:
                return target == _inventoryManager.chestSlot;

            case TypeItem.Legs:
                return target == _inventoryManager.legsSlot;

            case TypeItem.Boots:
                return target == _inventoryManager.bootsSlot;

            default:
                return false;
        }
    }

    private static void SwapInstanceSlots(itemSlot source, itemSlot target)
    {
        bool targetIsEmpty =
            !ItemInstance.IsReal(target.instance) &&
            target.quantity <= 0;

        if (!source.HasInstance ||
            (!target.HasInstance && !targetIsEmpty))
        {
            Debug.LogWarning(
                "Cannot mix ItemInstance data with a populated legacy slot."
            );

            return;
        }

        ItemInstance sourceInstance = source.instance;
        ItemInstance targetInstance = target.instance;

        target.SetInstance(sourceInstance);

        if (ItemInstance.IsReal(targetInstance))
            source.SetInstance(targetInstance);
        else
            source.Clear();

        Debug.Log(
            $"Moved ItemInstance '{sourceInstance.templateId}' " +
            $"to slot {target.slotID}."
        );
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

        Debug.Log(
            $"Moved {target.itemName} to slot {target.slotID}."
        );
    }
}