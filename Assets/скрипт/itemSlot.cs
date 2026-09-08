using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public enum SlotType
{
    Inventory,
    Equipment
}

public class itemSlot : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler
{
    public SlotType slotType;

    // Canonical data for new code. Legacy fields below remain a compatibility cache.
    public ItemInstance instance;


    public int slotID;
    public string itemName;
    public int quantity;
    public Sprite itemsprite;
    public bool ifFull;
   
    public Rarity rarity;
    public ItemAttributes attributes;

    [SerializeField]
   public TMP_Text quantityText;

    [SerializeField]
    public Image itemImage;

    [SerializeField]
    private Sprite defaultItemSprite;

    [SerializeField]
    private Sprite test;

    public GameObject SELECTEDsHADER;
    public bool thisItemSelected;
    private InventoryManager inventoryManager;
    public int maxNamberOfItem;
    private PlayerStatsUI playerStatsUI;

    private float clickTime = 0.0f;
    private float clickDelay = 0.3f; // Затримка для подвійного кліку

    private HoverdMouseDisplayInfo tooltip;
    public TypeItem itemType;

    public string itemDescription;//опис валюти
    public ItemData itemData;

    // ItemInstance is the source of truth for new code. These helpers fall back to
    // legacy fields only while InventoryManager still uses its legacy AddItem overload.
    public bool HasInstance => ItemInstance.IsReal(instance);

    public void SetInstance(ItemInstance newInstance)
    {
        if (newInstance == null)
        {
            Clear();
            return;
        }

        instance = newInstance;
        RefreshView();
    }

    public void Clear()
    {
        instance = null;
        itemName = null;
        quantity = 0;
        itemsprite = defaultItemSprite;
        ifFull = false;
        rarity = Rarity.Common;
        attributes = null;
        itemType = TypeItem.Misc;
        itemDescription = string.Empty;
        itemData = null;

        if (itemImage != null)
            itemImage.sprite = defaultItemSprite;

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.enabled = false;
        }

        if (SELECTEDsHADER != null)
            SELECTEDsHADER.SetActive(false);

        thisItemSelected = false;
    }

    public void RefreshView()
    {
        if (instance == null)
        {
            RefreshLegacyView();
            return;
        }

        ItemSO template;
        if (TryGetInstanceTemplate(out template))
        {
            itemName = template.itemName;
            itemsprite = template.sprite;
            itemDescription = template.description;
            itemType = template.typeItem;
            ifFull = instance.quantity >= (template.stackable ? Mathf.Max(1, template.maxStackSize) : 1);
        }
        else
        {
            // A missing catalog/template must not display stale data from a prior item.
            itemName = string.IsNullOrWhiteSpace(instance.itemName) ? instance.templateId : instance.itemName;
            itemsprite = defaultItemSprite;
            itemDescription = string.Empty;
            itemType = TypeItem.Misc;
            ifFull = instance.quantity > 0;
            Debug.LogWarning($"ItemSlot {slotID} could not resolve template '{instance.templateId}'.", this);
        }

        quantity = Mathf.Max(0, instance.quantity);
        rarity = instance.rarity;
        attributes = instance.attributes;
        itemData = new ItemData
        {
            itemID = instance.templateId,
            itemName = itemName,
            quantity = quantity,
            rarity = rarity,
            attributes = attributes,
            typeItem = itemType,
            description = itemDescription
        };

        RefreshLegacyView();
    }

    public string GetDisplayName()
    {
        ItemSO template;
        return instance != null && TryGetInstanceTemplate(out template) ? template.itemName : itemName;
    }

    public string GetDisplayDescription()
    {
        ItemSO template;
        return instance != null && TryGetInstanceTemplate(out template) ? template.description : itemDescription;
    }

    public TypeItem GetDisplayItemType()
    {
        ItemSO template;
        return instance != null && TryGetInstanceTemplate(out template) ? template.typeItem : itemType;
    }

    public int GetDisplayQuantity()
    {
        return instance != null ? instance.quantity : quantity;
    }

    public ItemAttributes GetDisplayAttributes()
    {
        return instance != null ? instance.attributes : attributes;
    }

    private bool TryGetInstanceTemplate(out ItemSO template)
    {
        template = null;
        return instance != null && ItemCatalog.Instance != null &&
               ItemCatalog.Instance.TryGetTemplate(instance.templateId, out template);
    }

    private void RefreshLegacyView()
    {
        if (itemImage != null)
            itemImage.sprite = itemsprite != null ? itemsprite : defaultItemSprite;

        if (quantityText != null)
        {
            quantityText.text = quantity > 0 ? quantity.ToString() : string.Empty;
            quantityText.enabled = quantity > 1;
        }
    }


    private void Awake()
    {
        DiscardDummyInstanceIfSlotEmpty();
        playerStatsUI = FindObjectOfType<PlayerStatsUI>(); // Ищем PlayerStatsUI при инициализации
    }

    private void DiscardDummyInstanceIfSlotEmpty()
    {
        if (instance == null || instance.HasIdentity || quantity > 0)
        {
            return;
        }

        instance = null;
    }
    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();

        tooltip = FindObjectOfType<HoverdMouseDisplayInfo>();

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        // Показувати підказку лише якщо слот є інвентарем і містить предмет
        if (slotType == SlotType.Inventory  && tooltip != null  )
        {
            if (quantity > 0 )
            {
                tooltip.ShowTooltip(this);

            }
        }
    }

    // Приховування підказки при знятті курсора
    public void OnPointerExit(PointerEventData eventData)
    {
        // Приховати підказку
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }
    public int AddItem (string itemName,
    int quantity,
    Sprite spriteItem,
    Rarity rarity,
    ItemAttributes attributes,
    TypeItem itemType,
    SlotType slotType)
    {
        // Legacy InventoryManager path: it owns the compatibility fields for now.
        instance = null;
        this.slotType = slotType;
        if (ifFull)
            return quantity;

        this.itemName = itemName;
        this.itemsprite = spriteItem;
        itemImage.sprite = spriteItem;
        this.rarity = rarity;
        this.attributes = attributes;
        this.itemType = itemType;

        this.quantity += quantity;
        if (this.quantity >= maxNamberOfItem)
        {
            quantityText.text = maxNamberOfItem.ToString();
            quantityText.enabled = true;
            ifFull = true;

            int extraItem = this.quantity - maxNamberOfItem;
            this.quantity = maxNamberOfItem;
            return extraItem;
        }

        quantityText.text = this.quantity.ToString();
        quantityText.enabled = this.quantity > 1;





        itemData = new ItemData();

        itemData.itemID = itemName; // тимчасово використовуємо назву як ID

        itemData.itemName = itemName;

        itemData.quantity = this.quantity;

        itemData.rarity = rarity;

        itemData.attributes = attributes;

        itemData.typeItem = itemType;

        itemData.description = itemDescription;





        return 0;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager>();

        Debug.Log("ВИ натиснули на слот з айді: " + slotID);

        // --- КРОК А: НАТИСКАННЯ ПРАВОЮ КНОПКОЮ МИШІ (ПКМ) ---
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Якщо у слоті є валюта — виділяємо її для крафту
            if (quantity > 0 && itemType == TypeItem.Currency)
            {
                // Якщо вже була виділена інша валюта — знімаємо з неї підсвітку
                if (inventoryManager.selectedItem != null && inventoryManager.selectedItem != this)
                {
                    if (inventoryManager.selectedItem.SELECTEDsHADER != null)
                        inventoryManager.selectedItem.SELECTEDsHADER.SetActive(false);
                    inventoryManager.selectedItem.thisItemSelected = false;
                }

                // Перемикаємо стан: якщо вона вже була виділена — скасовуємо виділення
                if (inventoryManager.selectedItem == this)
                {
                    inventoryManager.selectedItem = null;
                    thisItemSelected = false;
                    if (SELECTEDsHADER != null) SELECTEDsHADER.SetActive(false);
                    Debug.Log("[Валюта] Виділення скасовано.");
                }
                else // Якщо не була виділена — виділяємо
                {
                    inventoryManager.selectedItem = this;
                    thisItemSelected = true;
                    if (SELECTEDsHADER != null) SELECTEDsHADER.SetActive(true);
                    Debug.Log($"[Валюта] Активовано: {itemName}. Оберіть предмет для покращення ЛІВИМ кліком.");
                }

                return; // Зупиняємо виконання, щоб валюта НЕ викидалася!
            }

            // Якщо це звичайний предмет — викликаємо твою стандартну логіку (викинути/зняти)
            OnRigthClick();
        }

        // --- КРОК Б: НАТИСКАННЯ ЛІВОЮ КНОПКОЮ МИШІ (ЛКМ) ---
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Якщо ми ТРИМАЄМО валюту і клікаємо ЛКМ по іншому предмету — крафтимо!
            if (inventoryManager.selectedItem != null && inventoryManager.selectedItem.itemType == TypeItem.Currency)
            {
                // Перевіряємо, щоб ми не спробували застосувати валюту саму на себе
                if (inventoryManager.selectedItem == this)
                {
                    Debug.LogWarning("Не можна застосувати валюту саму на себе!");
                    return;
                }

                Debug.Log($"[Крафт] Застосовуємо {inventoryManager.selectedItem.itemName} на {this.itemName}");

                // Зберігаємо посилання на валюту, яка зараз крафтить
                itemSlot currentCurrency = inventoryManager.selectedItem;

                // Запускаємо твій метод крафту
                inventoryManager.ApplyCurrency(currentCurrency, this);

                // Після крафту знімаємо виділення, щоб можна було знову нормально клікати по предметах
                if (currentCurrency != null && currentCurrency.SELECTEDsHADER != null)
                {
                    currentCurrency.SELECTEDsHADER.SetActive(false);
                    currentCurrency.thisItemSelected = false;
                }
                inventoryManager.selectedItem = null;

                return; // Виходимо, щоб предмет випадково не вдягнувся під час крафту
            }

            // Якщо валюта НЕ виділена — викликаємо твій стандартний лівий клік (подвійний клік для екіпірування)
            OnLeftClick();
        }
    }


    public int GetSlotID()
    {
        return slotID;
    }
    public void RemoveItem()
    {
        Clear();
        Debug.Log("Slot cleared.");
    }

    public void OnLeftClick()
    {
        if (quantity <= 0)
        {
            return;
        }

        inventoryManager.DeselectAllSlot();
        SELECTEDsHADER.SetActive(true);
        thisItemSelected = true;
        
       
            LogItemInfo(); // Викликаємо метод для виводу інформації
        
        // Перевірка, чи предмет уже екіпірований
        if (ifFull && slotType == SlotType.Equipment)
        {
            Debug.Log($"Предмет {itemName} вже екіпірований, повторне натискання не викликатиме атрибути.");
            
            return; // Вийти, якщо предмет уже екіпірований
        }

        if (Time.time - clickTime < clickDelay)
        {
            // Перевірка, чи слот екіпіровки зайнятий, перед спробою екіпірувати предмет
            if (inventoryManager.helmetSlot.ifFull && itemName == "Helmet" ||
                inventoryManager.chestSlot.ifFull && itemName == "Chest" ||
                inventoryManager.legsSlot.ifFull && itemName == "Legs" ||
                inventoryManager.bootsSlot.ifFull && itemName == "Boots")
            {
                Debug.Log("Слот екіпіровки вже зайнятий. Спочатку зніміть предмет.");
                return;
            }

            inventoryManager.EquipItem(itemName, itemsprite, rarity, attributes, itemType, true, slotID); clickTime = 0f;
            Debug.Log($"ID слота: {slotID}, Спрайт предмета: {itemsprite}");
        }
        else
        {
            clickTime = Time.time; // Запам'ятовуємо час першого кліку
        }
    }



    // функція для зняття предмета зі слоту екіпірування 
    public void OnRigthClick()
    {
        
        // Перевіряємо, чи є поточний слот одним зі слотів екіпірування
        if (this == inventoryManager.helmetSlot || this == inventoryManager.chestSlot ||
            this == inventoryManager.legsSlot || this == inventoryManager.bootsSlot)
        {
            // Якщо слот заповнений, знімаємо предмет
            if (ifFull)
            {
                inventoryManager.UnequipItem(this); // Знімаємо предмет
            }
            else
            {
                Debug.Log("Слот порожній. Немає предмета для зняття.");
            }
        }
        else
        {
            Debug.Log("Цей слот не є слотом екіпірування.");
        }
        thisItemSelected = false; // 04 11 до цього працювало норм

        if (quantity > 0)
        {
            // Викинути всю кількість
            inventoryManager.DropItem(this, quantity);
        }
    }
    public void LogItemInfo()
    {
        // Якщо це валюта — виводимо лише назву, кількість та опис
        if (itemType == TypeItem.Currency)
        {
            string currencyInfo = $"=== ІНФОРМАЦІЯ ПРО ВАЛЮТУ ===\n" +
                                  $"Назва: {itemName}\n" +
                                  $"Кількість: {quantity}\n" +
                                  $"Опис: {itemDescription}";
            Debug.Log(currencyInfo);
            return; // Виходимо, щоб не друкувати пусті статы броні!
        }

        // Якщо це екіпірування (шолом, броня тощо) — виводимо статы як раніше
        if (attributes != null)
        {
            string info = $"=== Інформація про предмет ===\n" +
                          $"Назва: {itemName}\n" +
                          $"Кількість: {quantity}\n" +
                          $"Рідкість: {rarity}\n" +
                          $"Сила: {attributes.strength}\n" +
                          $"Спритність: {attributes.agility}\n" +
                          $"Інтелект: {attributes.intelligence}\n" +
                          $"Броня: {attributes.armour}\n" +
                          $"Якість: {attributes.quality}\n" +
                          $"Стійкість до вогню: {attributes.fireresist}\n" +
                          $"Стійкість до холоду: {attributes.coolresist}";
            Debug.Log(info);
        }
    }
}




