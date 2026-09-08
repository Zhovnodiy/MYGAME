using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemSO : ScriptableObject
{
    [Header("Template Identity")]
    [Tooltip("Stable ID used by ItemInstance and save data. Do not change it after release.")]
    public string templateId;

    [Header("Display")]
    public string itemName;
    [TextArea]
    public string description;
    public Sprite sprite;
    public TypeItem typeItem;

    [Header("Stacking")]
    public bool stackable;
    [Min(1)] public int maxStackSize = 1;

    [Header("World")]
    [Tooltip("Optional prefab used later when an ItemInstance is spawned in the world.")]
    public GameObject worldPrefab;

    [Header("Default Instance Settings")]
    public Rarity rarity = Rarity.Common;
    [Tooltip("When enabled, each new instance rolls attributes from the rarity ranges below.")]
    public bool generateRandomAttributes = true;

    [Header("Base Attributes")]
    public ItemAttributes baseAttributes = new ItemAttributes(0, 0, 0, 0, 0, 0, 0, 0, 0);

    [Header("Legacy Generated Attribute Fields")]
    // Kept for compatibility with existing assets. New code should use CreateAttributes().
    public int strength;
    public int agility;
    public int intelligence;
    public int armour;
    public int quality;
    public int coolResist;
    public int fireResist;
    public int lightningResist;
    public int darknessResist;
    public int attributes;

    /// <summary>
    /// Creates an independent runtime item from this immutable template.
    /// </summary>
    public ItemInstance CreateInstance(int quantity = 1)
    {
        return ItemInstance.Create(this, quantity);
    }

    /// <summary>
    /// Builds attributes for one item instance without mutating the template.
    /// </summary>
    public ItemAttributes CreateAttributes()
    {
        if (!generateRandomAttributes)
        {
            return CloneAttributes(baseAttributes);
        }

        switch (rarity)
        {
            case Rarity.Common:
                return new ItemAttributes(Random.Range(1, 5), Random.Range(1, 3), Random.Range(1, 2), Random.Range(1, 50), Random.Range(1, 5), Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7));
            case Rarity.Rare:
                return new ItemAttributes(Random.Range(6, 10), Random.Range(3, 6), Random.Range(2, 4), Random.Range(49, 75), Random.Range(4, 8), Random.Range(5, 10), Random.Range(5, 10), Random.Range(5, 10), Random.Range(5, 10));
            case Rarity.Epic:
                return new ItemAttributes(Random.Range(11, 15), Random.Range(7, 10), Random.Range(5, 8), Random.Range(78, 140), Random.Range(6, 17), Random.Range(10, 17), Random.Range(10, 17), Random.Range(10, 17), Random.Range(10, 17));
            case Rarity.Legendary:
                return new ItemAttributes(Random.Range(16, 20), Random.Range(11, 15), Random.Range(9, 12), Random.Range(150, 311), Random.Range(16, 26), Random.Range(17, 25), Random.Range(17, 25), Random.Range(17, 25), Random.Range(17, 25));
            default:
                return CloneAttributes(baseAttributes);
        }
    }

    private static ItemAttributes CloneAttributes(ItemAttributes source)
    {
        if (source == null)
        {
            return new ItemAttributes(0, 0, 0, 0, 0, 0, 0, 0, 0);
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

    // Legacy method retained for existing Inspector events or external calls.
    public void GenerateAttributes()
    {
        ItemAttributes generated = CreateAttributes();
        strength = generated.strength;
        agility = generated.agility;
        intelligence = generated.intelligence;
        armour = generated.armour;
        quality = generated.quality;
        coolResist = generated.coolresist;
        fireResist = generated.fireresist;
        lightningResist = generated.lightinresist;
        darknessResist = generated.darknessresist;
    }
}