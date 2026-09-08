using System;

[Serializable]
public class ItemInstance
{
    // Canonical runtime/save fields for a concrete item.
    public string guid;
    public string templateId;
    public int quantity = 1;
    public Rarity rarity;
    public ItemAttributes attributes;

    // Legacy fields are retained temporarily because existing scene/prefab data may contain them.
    public int itemID;
    public string itemName;
    public int stack = 1;

    // Dummy Unity-serialized instances have no templateId and must not count as real items.
    public bool HasIdentity => !string.IsNullOrWhiteSpace(templateId);

    public static bool IsReal(ItemInstance instance)
    {
        return instance != null && instance.HasIdentity && instance.quantity > 0;
    }

    public static ItemInstance Create(ItemSO template, int quantity = 1)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        if (string.IsNullOrWhiteSpace(template.templateId))
        {
            throw new ArgumentException("ItemSO must have a templateId before creating an ItemInstance.", nameof(template));
        }

        int normalizedQuantity = Math.Max(1, quantity);

        return new ItemInstance
        {
            guid = Guid.NewGuid().ToString("N"),
            templateId = template.templateId,
            quantity = normalizedQuantity,
            rarity = template.rarity,
            attributes = template.CreateAttributes(),

            // Temporary compatibility mirrors; future stages will stop reading them.
            itemName = template.itemName,
            stack = normalizedQuantity
        };
    }

    /// <summary>
    /// Creates a separate runtime item record with a new GUID. Used when a stack is split.
    /// </summary>
    public ItemInstance CloneWithQuantity(int newQuantity)
    {
        return new ItemInstance
        {
            guid = Guid.NewGuid().ToString("N"),
            templateId = templateId,
            quantity = Math.Max(1, newQuantity),
            rarity = rarity,
            attributes = CloneAttributes(attributes),

            // Temporary compatibility mirrors.
            itemID = itemID,
            itemName = itemName,
            stack = Math.Max(1, newQuantity)
        };
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
}
