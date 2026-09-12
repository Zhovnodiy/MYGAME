using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemSpriteCatalog : MonoBehaviour
{
    public static ItemSpriteCatalog Instance { get; private set; }

    [System.Serializable]
    public class SpriteEntry
    {
        public Sprite sprite;
    }

    [Header("Item Sprites")]
    public List<SpriteEntry> sprites = new List<SpriteEntry>();

    private Dictionary<string, Sprite> spritesByName =
        new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildIndex();
    }

    private void BuildIndex()
    {
        spritesByName.Clear();

        foreach (SpriteEntry entry in sprites)
        {
            if (entry == null || entry.sprite == null)
                continue;

            string key = entry.sprite.name;

            if (!spritesByName.ContainsKey(key))
            {
                spritesByName.Add(key, entry.sprite);
            }
        }

        Debug.Log($"ItemSpriteCatalog: loaded {spritesByName.Count} sprites.");
    }

    public Sprite GetSprite(string spriteName)
    {
        if (string.IsNullOrWhiteSpace(spriteName))
            return null;

        if (spritesByName.TryGetValue(spriteName, out Sprite sprite))
            return sprite;

        Debug.LogWarning(
            $"ItemSpriteCatalog: sprite not found: {spriteName}"
        );

        return null;
    }

#if UNITY_EDITOR

    [ContextMenu("Load All Sprites From 1024 Folder")]
    public void LoadAllSpritesFromFolder()
    {
        const string folderPath = "Assets/60 free icons pack/1024";

        string[] guids = AssetDatabase.FindAssets(
            "t:Sprite",
            new[] { folderPath }
        );

        List<SpriteEntry> loadedSprites = new List<SpriteEntry>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite == null)
                continue;

            SpriteEntry entry = new SpriteEntry();
            entry.sprite = sprite;

            loadedSprites.Add(entry);
        }

        loadedSprites.Sort((a, b) =>
            string.Compare(
                a.sprite.name,
                b.sprite.name,
                System.StringComparison.OrdinalIgnoreCase
            )
        );

        sprites = loadedSprites;

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log(
            $"ItemSpriteCatalog: automatically loaded {sprites.Count} sprites from {folderPath}"
        );
    }

#endif
}