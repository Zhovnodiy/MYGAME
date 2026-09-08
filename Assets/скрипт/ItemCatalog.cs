using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime lookup for immutable item templates. Unity object references stay here,
/// while save data refers to a template only by ItemSO.templateId.
/// </summary>
public class ItemCatalog : MonoBehaviour
{
    public static ItemCatalog Instance { get; private set; }

    [SerializeField] private ItemSO[] templates;

    private readonly Dictionary<string, ItemSO> templatesById = new Dictionary<string, ItemSO>(StringComparer.Ordinal);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple ItemCatalog instances found. The later instance will be ignored.", this);
            return;
        }

        Instance = this;
        RebuildIndex();
    }

    public bool TryGetTemplate(string templateId, out ItemSO template)
    {
        if (string.IsNullOrWhiteSpace(templateId))
        {
            template = null;
            return false;
        }

        return templatesById.TryGetValue(templateId, out template);
    }

    public ItemSO GetTemplate(string templateId)
    {
        if (TryGetTemplate(templateId, out ItemSO template))
        {
            return template;
        }

        Debug.LogWarning($"Item template not found: '{templateId}'.", this);
        return null;
    }

    /// <summary>
    /// Temporary bridge for callers that still provide legacy item fields instead of an ItemSO.
    /// The created template lives only in memory and is never save data.
    /// </summary>
    public ItemSO GetOrCreateLegacyTemplate(
        string itemName,
        string description,
        Sprite sprite,
        TypeItem typeItem,
        Rarity rarity,
        ItemAttributes attributes,
        int maxStackSize)
    {
        string legacyId = $"legacy:{typeItem}:{itemName}:{(sprite != null ? sprite.name : "no-sprite")}";
        if (TryGetTemplate(legacyId, out ItemSO existingTemplate))
        {
            return existingTemplate;
        }

        ItemSO legacyTemplate = ScriptableObject.CreateInstance<ItemSO>();
        legacyTemplate.hideFlags = HideFlags.DontSave;
        legacyTemplate.name = legacyId;
        legacyTemplate.templateId = legacyId;
        legacyTemplate.itemName = itemName;
        legacyTemplate.description = description;
        legacyTemplate.sprite = sprite;
        legacyTemplate.typeItem = typeItem;
        legacyTemplate.rarity = rarity;
        legacyTemplate.stackable = true;
        legacyTemplate.maxStackSize = Mathf.Max(1, maxStackSize);
        legacyTemplate.generateRandomAttributes = false;
        legacyTemplate.baseAttributes = attributes;

        templatesById.Add(legacyId, legacyTemplate);
        return legacyTemplate;
    }

    public void RebuildIndex()
    {
        templatesById.Clear();

        if (templates == null)
        {
            return;
        }

        foreach (ItemSO template in templates)
        {
            if (template == null || string.IsNullOrWhiteSpace(template.templateId))
            {
                continue;
            }

            if (templatesById.ContainsKey(template.templateId))
            {
                Debug.LogError($"Duplicate ItemSO templateId: '{template.templateId}'.", template);
                continue;
            }

            templatesById.Add(template.templateId, template);
        }
    }
}
