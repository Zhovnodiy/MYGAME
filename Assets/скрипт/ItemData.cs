using System;

[Serializable]
public class ItemData
{
    public string itemID;

    public string itemName;

    public int quantity;

    public Rarity rarity;

    public ItemAttributes attributes;

    public TypeItem typeItem;

    public string description;
}