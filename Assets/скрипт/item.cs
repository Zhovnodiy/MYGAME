using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
public enum TypeItem
{
    Helmet,
    Chest,
    Legs,
    Boots,

    Weapon,
    Shield,

    Potion,
    Ore,
    Currency,

    Misc

}

[System.Serializable]
public class ItemPrefixes
{
    public int strength;
    public int agility;
    public int intelligence;
    public int armour; // Броня предмета
}

[System.Serializable]
public class ItemSuffixes
{
    public int coolresist;
    public int fireresist;
    public int lightinresist;
    public int darknessresist;
}




[System.Serializable]
public class ItemAttributes
{
    public int quality;

    public ItemPrefixes prefixes = new ItemPrefixes();
    public ItemSuffixes suffixes = new ItemSuffixes();

    [SerializeField]
    private bool hasFixedAttributes; // Додано поле для визначення, чи будуть характеристики фіксованими

    [SerializeField]
    private ItemAttributes fixedAttributes;

    public int strength { get => prefixes.strength; set => prefixes.strength = value; }
    public int agility { get => prefixes.agility; set => prefixes.agility = value; }
    public int intelligence { get => prefixes.intelligence; set => prefixes.intelligence = value; }
    public int armour { get => prefixes.armour; set => prefixes.armour = value; }

    public int coolresist { get => suffixes.coolresist; set => suffixes.coolresist = value; }
    public int fireresist { get => suffixes.fireresist; set => suffixes.fireresist = value; }
    public int lightinresist { get => suffixes.lightinresist; set => suffixes.lightinresist = value; }
    public int darknessresist { get => suffixes.darknessresist; set => suffixes.darknessresist = value; }

    public ItemAttributes(int strength, int agility, int intelligence, int armour, int quality,int coolresist,int fireresist,int lightinresist,int darknessresist)
    {
        this.strength = strength;
        this.agility = agility;
        this.intelligence = intelligence;
        this.armour = armour;
        this.quality = quality;
        this.coolresist = coolresist;
        this.fireresist = fireresist;
        this.lightinresist = lightinresist;
        this.darknessresist = darknessresist;
    }
}

public class item : MonoBehaviour
{
    [SerializeField]
   public bool useFixedAttributes; // Чи використовувати фіксовані атрибути

    [SerializeField]
   public ItemAttributes fixedAttributes; // Фіксовані атрибути

    [SerializeField]
    public string itemName;
    [Header("Database")]
    public string itemID;

    [SerializeField]
    public int quantity;

    [SerializeField]
   public Sprite sprite;

   public InventoryManager inventoryManager;
    public ItemAttributes attributes;

    [SerializeField]

    public Rarity rarity;
    public TypeItem typeItem;

    public string itemDescription;
    public ItemInstance instance;






    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        attributes = GenerateAttributesByRarity(rarity);
    }

    private ItemAttributes GenerateAttributesByRarity(Rarity rarity)
    {
        if (useFixedAttributes)
        {
            return fixedAttributes;
        }

        switch (rarity)
        {
            case Rarity.Common:
                return new ItemAttributes(Random.Range(1, 5), Random.Range(1, 3), Random.Range(1, 2),Random.Range(1, 50), Random.Range(1, 5), Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7));
            case Rarity.Rare:
                return new ItemAttributes(Random.Range(6, 10), Random.Range(3, 6), Random.Range(2, 4),Random.Range(49, 75),Random.Range(4, 8), Random.Range(5, 10), Random.Range(5, 10), Random.Range(5, 10), Random.Range(5, 10));
            case Rarity.Epic:
                return new ItemAttributes(Random.Range(11, 15), Random.Range(7, 10), Random.Range(5, 8),Random.Range(78, 140), Random.Range(6, 17), Random.Range(10, 17), Random.Range(10, 17), Random.Range(10, 17), Random.Range(10, 17));
            case Rarity.Legendary:
                return new ItemAttributes(Random.Range(16, 20), Random.Range(11, 15), Random.Range(9, 12),Random.Range(150, 311), Random.Range(16, 26), Random.Range(17, 25), Random.Range(17, 25), Random.Range(17, 25), Random.Range(17, 25));
            default:
                return new ItemAttributes(1, 1, 1,1,1,1,1,1,1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player collided with item: " + itemName + " (Rarity: " + rarity + ")");
            Debug.Log($"Attributes - Strength: {attributes.strength}, Agility: {attributes.agility}, Intelligence: {attributes.intelligence}, Armour {attributes.armour},Quality {attributes.quality},fire {attributes.fireresist},cool{attributes.coolresist},light{attributes.lightinresist},darkness{attributes.darknessresist}");

            int leftOverItem = inventoryManager.AddItem(itemName, quantity, sprite, rarity, attributes, typeItem, itemDescription);
            if (leftOverItem <= 0)
                Destroy(gameObject);
            else
                quantity = leftOverItem;
        }
    }

    // Метод, який викликається при наведені курсора на предмет
    private void OnMouseEnter()
    {
       // Debug.Log("Навели на предмет: " + itemName);
    }

    // Метод, який викликається, коли курсор виходить з предмета
    private void OnMouseExit()
    {
      //  Debug.Log("Вийшли з предмета: " + itemName);
    }

    void Update()
    {

    }
    public ItemData GetItemData()
    {
        ItemData data = new ItemData();

        data.itemID = itemID;
        data.itemName = itemName;
        data.quantity = quantity;
        data.rarity = rarity;
        data.attributes = attributes;
        data.typeItem = typeItem;
        data.description = itemDescription;

        return data;
    }
}
