using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool MenuActivate;
    public itemSlot[] itemSlot;
    public itemSlot helmetSlot;
    public itemSlot chestSlot;
    public itemSlot legsSlot;
    public itemSlot bootsSlot;
    public itemSlot selectedItem;

    private void Start()
    {
        EnsureCatalog();
        InventoryMenu.SetActive(false);
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].slotID = i;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && MenuActivate)
        {
            Time.timeScale = 1;
            InventoryMenu.SetActive(false);
            MenuActivate = false;
        }
        else if (Input.GetKeyUp(KeyCode.E) && !MenuActivate)
        {
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            MenuActivate = true;
        }
    }

    // Canonical add path for new code.
    public int AddItem(ItemInstance newInstance)
    {
        if (newInstance == null || newInstance.quantity <= 0)
        {
            return 0;
        }

        ItemCatalog catalog = EnsureCatalog();
        if (!catalog.TryGetTemplate(newInstance.templateId, out ItemSO template))
        {
            Debug.LogWarning($"Cannot add ItemInstance: template '{newInstance.templateId}' was not found.");
            return newInstance.quantity;
        }

        int remaining = newInstance.quantity;
        int initialQuantity = remaining;
        int maxStackSize = template.stackable ? Mathf.Max(1, template.maxStackSize) : 1;

        if (template.stackable)
        {
            for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
            {
                itemSlot slot = itemSlot[i];
                if (!CanStack(slot, newInstance, template))
                {
                    continue;
                }

                int space = maxStackSize - slot.instance.quantity;
                if (space <= 0)
                {
                    continue;
                }

                int amountToMove = Mathf.Min(space, remaining);
                slot.instance.quantity += amountToMove;
                slot.instance.stack = slot.instance.quantity;
                slot.RefreshView();
                remaining -= amountToMove;
            }
        }

        while (remaining > 0)
        {
            itemSlot emptySlot = FindEmptyInventorySlot();
            if (emptySlot == null)
            {
                newInstance.quantity = remaining;
                newInstance.stack = remaining;
                return remaining;
            }

            int amountForSlot = Mathf.Min(maxStackSize, remaining);
            ItemInstance storedInstance = remaining == initialQuantity
                ? newInstance
                : newInstance.CloneWithQuantity(amountForSlot);

            storedInstance.quantity = amountForSlot;
            storedInstance.stack = amountForSlot;
            emptySlot.slotType = SlotType.Inventory;
            emptySlot.SetInstance(storedInstance);
            remaining -= amountForSlot;
        }

        return 0;
    }

    // Legacy compatibility wrapper used by the current world-item and shop code.
    public int AddItem(string itemName, int quantity, Sprite spriteItem, Rarity rarity, ItemAttributes attributes, TypeItem itemType, string itemDescription = "")
    {
        if (quantity <= 0)
        {
            return 0;
        }

        ItemCatalog catalog = EnsureCatalog();
        ItemSO template = catalog.GetOrCreateLegacyTemplate(
            itemName,
            itemDescription,
            spriteItem,
            itemType,
            rarity,
            attributes,
            GetLegacyMaxStackSize());

        ItemInstance newInstance = ItemInstance.Create(template, quantity);
        newInstance.rarity = rarity;
        newInstance.attributes = CloneAttributes(attributes);
        newInstance.itemName = itemName;
        return AddItem(newInstance);
    }

    public void DeselectAllSlot()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].SELECTEDsHADER != null)
                itemSlot[i].SELECTEDsHADER.SetActive(false);

            itemSlot[i].thisItemSelected = false;
        }

        selectedItem = null;
    }

    // Existing public signature retained for UI and legacy callers.
    public void EquipItem(string itemName, Sprite spriteItem, Rarity rarity, ItemAttributes attributes, TypeItem itemType, bool v, int slotID)
    {
        itemSlot sourceSlot = FindInventorySlotById(slotID);
        if (sourceSlot == null || !ItemInstance.IsReal(sourceSlot.instance))
        {
            Debug.LogWarning($"Cannot equip legacy item '{itemName}': source slot {slotID} has no ItemInstance.");
            return;
        }

        EquipItem(sourceSlot);
    }

    // Canonical equipment path. TypeItem determines the destination slot.
    public bool EquipItem(itemSlot sourceSlot)
    {
        if (sourceSlot == null || !ItemInstance.IsReal(sourceSlot.instance))
        {
            return false;
        }

        if (!TryGetTemplate(sourceSlot.instance, out ItemSO template))
        {
            return false;
        }

        itemSlot targetSlot = GetEquipmentSlot(template.typeItem);
        if (targetSlot == null)
        {
            Debug.Log($"No equipment slot exists for item type {template.typeItem}.");
            return false;
        }

        if (targetSlot == sourceSlot)
        {
            return false;
        }

        ItemInstance previouslyEquipped = ItemInstance.IsReal(targetSlot.instance) ? targetSlot.instance : null;
        ItemInstance incoming = sourceSlot.instance;
        bool splitFromStack = incoming.quantity > 1;

        ItemInstance equippedInstance;
        if (splitFromStack)
        {
            incoming.quantity--;
            incoming.stack = incoming.quantity;
            sourceSlot.RefreshView();
            equippedInstance = incoming.CloneWithQuantity(1);
        }
        else
        {
            equippedInstance = incoming;
            sourceSlot.Clear();
        }

        if (previouslyEquipped != null)
        {
            int oldItemLeftOver = AddItem(previouslyEquipped);
            if (oldItemLeftOver > 0)
            {
                if (splitFromStack)
                {
                    incoming.quantity++;
                    incoming.stack = incoming.quantity;
                    sourceSlot.RefreshView();
                }
                else
                {
                    sourceSlot.slotType = SlotType.Inventory;
                    sourceSlot.SetInstance(equippedInstance);
                }

                Debug.Log("Equipment replacement cancelled because the inventory has no space for the equipped item.");
                return false;
            }

            PlayerStatsUI unequipStatsUI = FindObjectOfType<PlayerStatsUI>();
            if (unequipStatsUI != null)
                unequipStatsUI.UpdateStats(previouslyEquipped.attributes, false);
        }

        equippedInstance.quantity = 1;
        equippedInstance.stack = 1;
        targetSlot.slotType = SlotType.Equipment;
        targetSlot.SetInstance(equippedInstance);

        PlayerStatsUI playerStatsUI = FindObjectOfType<PlayerStatsUI>();
        if (playerStatsUI != null)
        {
            playerStatsUI.UpdateStats(equippedInstance.attributes, true);
        }
        else
        {
            Debug.LogError("PlayerStatsUI was not found.");
        }

        Debug.Log($"Equipped {template.itemName} in {targetSlot.slotType}.");
        return true;
    }

 

    public void DropItem(itemSlot slot, int quantityToDrop)
    {
        if (slot == null || !ItemInstance.IsReal(slot.instance) || quantityToDrop <= 0)
        {
            return;
        }

        int amountToDrop = Mathf.Min(quantityToDrop, slot.instance.quantity);
        if (amountToDrop <= 0)
        {
            return;
        }

        ItemInstance sourceInstance = slot.instance;
        ItemInstance droppedInstance = amountToDrop == sourceInstance.quantity
            ? sourceInstance
            : sourceInstance.CloneWithQuantity(amountToDrop);

        string displayName = slot.GetDisplayName();
        string displayDescription = slot.GetDisplayDescription();
        Sprite displaySprite = slot.itemsprite;
        TypeItem displayType = slot.GetDisplayItemType();

        if (amountToDrop == sourceInstance.quantity)
        {
            slot.Clear();
        }
        else
        {
            sourceInstance.quantity -= amountToDrop;
            sourceInstance.stack = sourceInstance.quantity;
            slot.RefreshView();
        }

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 dropPosition = playerTransform.position + playerTransform.right * 1.5f;

        GameObject droppedItem = new GameObject(displayName);
        droppedItem.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        droppedItem.transform.position = dropPosition;

        SpriteRenderer spriteRenderer = droppedItem.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = displaySprite;
        spriteRenderer.sortingOrder = 1;

        BoxCollider2D collider = droppedItem.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        item itemComponent = droppedItem.AddComponent<item>();
        itemComponent.instance = droppedInstance;
        itemComponent.itemID = droppedInstance.templateId;
        itemComponent.itemName = displayName;
        itemComponent.itemDescription = displayDescription;
        itemComponent.quantity = droppedInstance.quantity;
        itemComponent.sprite = displaySprite;
        itemComponent.rarity = droppedInstance.rarity;
        itemComponent.useFixedAttributes = true;
        itemComponent.fixedAttributes = droppedInstance.attributes;
        itemComponent.typeItem = displayType;

        Debug.Log($"Dropped {amountToDrop} {displayName} at {dropPosition}.");
    }

    public void ApplyCurrency(itemSlot currencySlot, itemSlot targetSlot)
    {
        if (currencySlot == null || targetSlot == null ||
            !ItemInstance.IsReal(currencySlot.instance) || !ItemInstance.IsReal(targetSlot.instance))
        {
            Debug.LogWarning("Currency application requires two ItemInstance slots.");
            return;
        }

        ItemInstance currencyInstance = currencySlot.instance;
        ItemInstance targetInstance = targetSlot.instance;
        TypeItem targetType = targetSlot.GetDisplayItemType();

        if (currencySlot.GetDisplayItemType() != TypeItem.Currency)
        {
            Debug.LogWarning("Selected item is not currency.");
            return;
        }

        if (targetType == TypeItem.Currency || targetType == TypeItem.Potion || targetType == TypeItem.Ore || targetType == TypeItem.Misc)
        {
            Debug.LogWarning("Currency cannot be applied to this item type.");
            return;
        }

        if (targetInstance.attributes == null)
        {
            Debug.LogError("Target ItemInstance has no attributes.");
            return;
        }

        string currencyName = currencySlot.GetDisplayName().ToLowerInvariant();
        bool success = false;

        if (currencyName.Contains("chaos") || currencyName.Contains("хаос"))
        {
            targetInstance.attributes.prefixes.strength = UnityEngine.Random.Range(1, 15);
            targetInstance.attributes.prefixes.agility = UnityEngine.Random.Range(1, 15);
            targetInstance.attributes.prefixes.intelligence = UnityEngine.Random.Range(1, 15);
            targetInstance.attributes.prefixes.armour = UnityEngine.Random.Range(5, 50);
            targetInstance.attributes.suffixes.fireresist = UnityEngine.Random.Range(5, 30);
            targetInstance.attributes.suffixes.coolresist = UnityEngine.Random.Range(5, 30);
            targetInstance.attributes.suffixes.lightinresist = UnityEngine.Random.Range(5, 30);
            targetInstance.attributes.suffixes.darknessresist = UnityEngine.Random.Range(5, 30);
            targetInstance.rarity = (Rarity)UnityEngine.Random.Range(0, 4);
            success = true;
        }
        else if (currencyName.Contains("whetstone") || currencyName.Contains("точиль") || currencyName.Contains("скрап"))
        {
            if (targetInstance.attributes.quality < 20)
            {
                targetInstance.attributes.quality = Mathf.Min(targetInstance.attributes.quality + 5, 20);
                targetInstance.attributes.armour = Mathf.RoundToInt(targetInstance.attributes.armour * 1.05f);
                success = true;
            }
            else
            {
                Debug.LogWarning("Item quality is already at its maximum.");
            }
        }

        if (!success)
        {
            return;
        }

        currencyInstance.quantity--;
        currencyInstance.stack = currencyInstance.quantity;
        if (currencyInstance.quantity <= 0)
        {
            currencySlot.Clear();
            selectedItem = null;
        }
        else
        {
            currencySlot.RefreshView();
        }

        targetSlot.RefreshView();
        targetSlot.LogItemInfo();
    }

    private ItemCatalog EnsureCatalog()
    {
        if (ItemCatalog.Instance != null)
        {
            return ItemCatalog.Instance;
        }

        GameObject catalogObject = new GameObject("Runtime ItemCatalog");
        return catalogObject.AddComponent<ItemCatalog>();
    }

    private bool TryGetTemplate(ItemInstance instance, out ItemSO template)
    {
        template = null;
        return instance != null && EnsureCatalog().TryGetTemplate(instance.templateId, out template);
    }

    private itemSlot FindEmptyInventorySlot()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot slot = itemSlot[i];
            if (slot == null)
            {
                continue;
            }

            bool noRealInstance = slot.instance == null
                || string.IsNullOrWhiteSpace(slot.instance.templateId)
                || slot.instance.quantity <= 0;

            if (noRealInstance && slot.quantity <= 0)
            {
                return slot;
            }
        }

        return null;
    }

    private itemSlot FindInventorySlotById(int slotID)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].slotID == slotID)
            {
                return itemSlot[i];
            }
        }

        return null;
    }

    private itemSlot GetEquipmentSlot(TypeItem typeItem)
    {
        switch (typeItem)
        {
            case TypeItem.Helmet:
                return helmetSlot;
            case TypeItem.Chest:
                return chestSlot;
            case TypeItem.Legs:
                return legsSlot;
            case TypeItem.Boots:
                return bootsSlot;
            default:
                return null;
        }
    }

    private bool CanStack(itemSlot slot, ItemInstance incoming, ItemSO template)
    {
        return slot != null && ItemInstance.IsReal(slot.instance) && template.stackable &&
               slot.instance.templateId == incoming.templateId &&
               slot.instance.rarity == incoming.rarity &&
               AttributesMatch(slot.instance.attributes, incoming.attributes);
    }

    private int GetLegacyMaxStackSize()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] != null && itemSlot[i].maxNamberOfItem > 0)
            {
                return itemSlot[i].maxNamberOfItem;
            }
        }

        return 1;
    }

    private static bool AttributesMatch(ItemAttributes first, ItemAttributes second)
    {
        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (first == null || second == null)
        {
            return false;
        }

        return first.strength == second.strength &&
               first.agility == second.agility &&
               first.intelligence == second.intelligence &&
               first.armour == second.armour &&
               first.quality == second.quality &&
               first.coolresist == second.coolresist &&
               first.fireresist == second.fireresist &&
               first.lightinresist == second.lightinresist &&
               first.darknessresist == second.darknessresist;
    }

    private static ItemAttributes CloneAttributes(ItemAttributes source)
    {
        if (source == null)
        {
            return null;
        }

        return new ItemAttributes(
            source.strength,
            source.agility,
            source.intelligence,
            source.armour,
            source.quality,
            source.coolresist,
            source.fireresist,
            source.lightinresist,
            source.darknessresist);
    }

    public void UnequipItem(itemSlot slot)
    {
        // Перевіряємо, чи слот не порожній
        if (!slot.ifFull)
        {
            Debug.Log("Слот порожній. Немає предмета для зняття.");
            return;
        }

        // Створюємо новий ItemInstance для додавання в інвентар
        ItemSO template = EnsureCatalog().GetTemplate(slot.itemName);
        if (template == null)
        {
            Debug.LogError("Не вдалося знайти шаблон предмета для зняття.");
            return;
        }

        // Створюємо екземпляр предмета з тими самими атрибутами
        ItemInstance newItemInstance = new ItemInstance
        {
            guid = Guid.NewGuid().ToString("N"),
            templateId = slot.itemName,
            quantity = 1,
            rarity = slot.rarity,
            attributes = slot.attributes,
            
            // Копіюємо значення з старого екземпляра для сумісності
            itemName = slot.itemName,
            stack = 1
        };

        // Додаємо предмет у інвентар
        int remaining = AddItem(newItemInstance);
        
        // Якщо не вдалося додати всі предмети (інвентар заповнений)
        if (remaining > 0)
        {
            Debug.Log("Інвентар заповнений. Предмет залишився у слоті екіпірування.");
            return;
        }

        // Віднімаємо атрибути від гравця
        PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.strength -= slot.attributes.strength;
            playerStats.agility -= slot.attributes.agility;
            playerStats.intelligence -= slot.attributes.intelligence;
            playerStats.armour -= slot.attributes.armour;
        }

        // Очищуємо слот екіпірування
        slot.Clear();
    }
}
