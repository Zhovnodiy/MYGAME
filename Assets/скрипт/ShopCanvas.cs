using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvas : MonoBehaviour
{
    [Header("Main Menu Setup")]
    public GameObject ShopMenu;
    public Text[] CatalogsName;
    public Color notSelect = Color.white;
    public Color selected = Color.green;
    public AudioSource catalogmenu;
    public string KatalogName;

    [Header("Item Info UI")]
    public GameObject ItemTitle;
    public TMP_Text BuyitemNameText;
    public TMP_Text BuystrengthText;
    public TMP_Text BuyagilityText;
    public TMP_Text BuyintelligenceText;
    public TMP_Text BuyArmour;
    public TMP_Text BuyQality;
    public TMP_Text BuyFire_RES;
    public TMP_Text BuyColl_RES;
    public TMP_Text BuyLight_RES;
    public TMP_Text SetComplect;
    public Sprite itemImage;

    [Header("Categories GameObjects")]
    public GameObject catecoriaAll;
    public GameObject catecoriaWeapon;
    public GameObject catecoriaFood;
    public GameObject catecoriaPotion;
    public GameObject catecoriaArmour;
    public GameObject catecoriaAparell;

    [Header("Shop Settings")]
    public string targetTag = "quantity";
    public GameObject[] towarButtons;

    [Header("Currency")]
    public int coper = 10;
    public int silver = 10;
    public int gold = 10;

    public Text GoldenCoin;
    public Text coperCoin;
    public Text SilverCOin;

    [Header("Item Spawning (Fallback)")]
    public GameObject prefabToSpawn;
    public GameObject prefabToSpawn2;
    public float spawnOffset = 0f;
    public Transform player;

    private int selectedIndex = 0;
    private bool AcrivityShopMenu;
    private InventoryManager inventorymanager; // Має точно збігатися з назвою змінної у вашому початковому коді

    void Start()
    {
        inventorymanager = FindObjectOfType<InventoryManager>();

        UpdateCurrencyUI();
        InitializeButtons();

        UpdateMenu();
        ShopMenu.SetActive(false);
        AcrivityShopMenu = false;

        if (catalogmenu != null)
        {
            catalogmenu.volume = 0;
            catalogmenu.Play();
        }

        CheckWhyCatalogName();
    }

    private void InitializeButtons()
    {
        if (towarButtons == null) return;

        foreach (var button in towarButtons)
        {
            if (button != null)
            {
                Button btn = button.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnTowarButtonClicked(button));
                }
            }
        }
    }

    void OnTowarButtonClicked(GameObject button)
    {
        if (button == null) return;

        TowarInfoItems towarInfo = button.GetComponent<TowarInfoItems>();
        if (towarInfo == null) return;

        towarInfo.DisplayItemInfo();
        Debug.Log("itemId: " + towarInfo.itemId);

        if (BuyitemNameText != null) BuyitemNameText.text = towarInfo.itemName;
        if (BuystrengthText != null) BuystrengthText.text = $"+ {towarInfo.BuystrengthText} To Strength";
        if (BuyagilityText != null) BuyagilityText.text = $"+ {towarInfo.BuyagilityText} To Agility";
        if (BuyintelligenceText != null) BuyintelligenceText.text = $"+ {towarInfo.BuyintelligenceText} To Intelligence";
        if (BuyArmour != null) BuyArmour.text = towarInfo.BuyArmour.ToString();
        if (BuyQality != null) BuyQality.text = towarInfo.BuyQality.ToString();
        if (BuyFire_RES != null) BuyFire_RES.text = $"+ {towarInfo.BuyFire_RES}% To Fire Resistance";
        if (BuyColl_RES != null) BuyColl_RES.text = $"+ {towarInfo.BuyColl_RES}% To Cold Resistance";
        if (BuyLight_RES != null) BuyLight_RES.text = $"+ {towarInfo.BuyLight_RES}% To Lightning Resistance";
        if (SetComplect != null) SetComplect.text = towarInfo.SetComplect.ToString();

        CheckItemDetailsById(towarInfo.itemId);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            AcrivityShopMenu = !AcrivityShopMenu;
            Time.timeScale = AcrivityShopMenu ? 0 : 1;
            ShopMenu.SetActive(AcrivityShopMenu);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeCategorySelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeCategorySelection(-1);
        }
    }

    private void ChangeCategorySelection(int direction)
    {
        if (CatalogsName == null || CatalogsName.Length == 0) return;

        if (catalogmenu != null)
        {
            catalogmenu.volume = 1;
            catalogmenu.Play();
        }

        selectedIndex = (selectedIndex + direction + CatalogsName.Length) % CatalogsName.Length;
        UpdateMenu();
        CheckWhyCatalogName();
    }

    public void UpdateMenu()
    {
        if (CatalogsName == null) return;

        for (int i = 0; i < CatalogsName.Length; i++)
        {
            if (CatalogsName[i] != null)
            {
                CatalogsName[i].color = (i == selectedIndex) ? selected : notSelect;
            }
        }
    }

    public void CheckWhyCatalogName()
    {
        switch (selectedIndex)
        {
            case 0: KatalogName = "All"; break;
            case 1: KatalogName = "Weapon"; break;
            case 2: KatalogName = "Food"; break;
            case 3: KatalogName = "Potion"; break;
            case 4: KatalogName = "Ore"; break;
            case 5: KatalogName = "Armour"; break;
            default: KatalogName = "All"; break;
        }

        if (catecoriaAll != null) catecoriaAll.SetActive(selectedIndex == 0);
        if (catecoriaWeapon != null) catecoriaWeapon.SetActive(selectedIndex == 1);
        if (catecoriaFood != null) catecoriaFood.SetActive(selectedIndex == 2);
        if (catecoriaPotion != null) catecoriaPotion.SetActive(selectedIndex == 3);
        if (catecoriaAparell != null) catecoriaAparell.SetActive(selectedIndex == 4);
        if (catecoriaArmour != null) catecoriaArmour.SetActive(selectedIndex == 5);

        Debug.Log("Ви обрали категорію: " + KatalogName);
    }

    public void BuyItem(TowarInfoItems towarInfo)
    {
        if (towarInfo == null) return;

        bool canBuy = false;

        switch (towarInfo.MoneyType)
        {
            case TypeMoney.Gold:
                if (gold >= towarInfo.itemPrice) canBuy = true;
                break;
            case TypeMoney.Coper:
                if (coper >= towarInfo.itemPrice) canBuy = true;
                break;
            case TypeMoney.Silver:
                if (silver >= towarInfo.itemPrice) canBuy = true;
                break;
        }

        if (!canBuy)
        {
            Debug.Log("Недостатньо коштів для купівлі: " + towarInfo.itemName);
            return;
        }

        if (towarInfo.itemPrefab == null)
        {
            Debug.LogError("Помилка: У товару відсутній префаб предмета!");
            return;
        }

        item itemComponent = towarInfo.itemPrefab.GetComponent<item>();
        Sprite itemSprite = itemComponent != null ? itemComponent.sprite : null;
        Rarity itemRarity = itemComponent != null ? itemComponent.rarity : Rarity.Common;

        // РОЗУМНЕ ВИЗНАЧЕННЯ ТИПУ ПРЕДМЕТА ПІД ВАШ ENUM:
        TypeItem itemType = TypeItem.Misc;

        string nameLower = towarInfo.itemName.ToLower();
        if (nameLower.Contains("helmet") || nameLower.Contains("шлем") || nameLower.Contains("каска"))
        {
            itemType = TypeItem.Helmet;
        }
        else if (nameLower.Contains("chest") || nameLower.Contains("armor") || nameLower.Contains("броня") || nameLower.Contains("нагрудник"))
        {
            itemType = TypeItem.Chest;
        }
        else if (nameLower.Contains("legs") || nameLower.Contains("pants") || nameLower.Contains("штани"))
        {
            itemType = TypeItem.Legs;
        }
        else if (nameLower.Contains("boots") || nameLower.Contains("shoes") || nameLower.Contains("черевики"))
        {
            itemType = TypeItem.Boots;
        }
        else if (KatalogName == "Weapon")
        {
            itemType = TypeItem.Weapon;
        }
        else if (KatalogName == "Potion")
        {
            itemType = TypeItem.Potion;
        }
        else if (KatalogName == "Ore")
        {
            itemType = TypeItem.Ore;
        }

        int strength = Convert.ToInt32(towarInfo.BuystrengthText);
        int agility = Convert.ToInt32(towarInfo.BuyagilityText);
        int intelligence = Convert.ToInt32(towarInfo.BuyintelligenceText);
        int armour = Convert.ToInt32(towarInfo.BuyArmour);
        int quality = Convert.ToInt32(towarInfo.BuyQality);
        int fireRes = Convert.ToInt32(towarInfo.BuyFire_RES);
        int coldRes = Convert.ToInt32(towarInfo.BuyColl_RES);
        int lightRes = Convert.ToInt32(towarInfo.BuyLight_RES);

        ItemAttributes shopAttributes = new ItemAttributes(strength, agility, intelligence, armour, quality, coldRes, fireRes, lightRes, 0);

        if (inventorymanager == null)
        {
            inventorymanager = FindObjectOfType<InventoryManager>();
        }

        if (inventorymanager == null)
        {
            Debug.LogError("Помилка: На сцені не знайдено InventoryManager!");
            return;
        }

        // Передаємо всі потрібні 6 параметрів у метод інвентаря
        int leftOver = inventorymanager.AddItem(towarInfo.itemName, 1, itemSprite, itemRarity, shopAttributes, itemType);

        if (leftOver == 0)
        {
            switch (towarInfo.MoneyType)
            {
                case TypeMoney.Gold: gold -= (int)towarInfo.itemPrice; break;
                case TypeMoney.Coper: coper -= (int)towarInfo.itemPrice; break;
                case TypeMoney.Silver: silver -= (int)towarInfo.itemPrice; break;
            }

            UpdateCurrencyUI();
            Debug.Log("Предмет успішно куплено та додано в інвентар: " + towarInfo.itemName);
        }
        else
        {
            Debug.LogWarning("Неможливо купити предмет: Інвентар повний!");
        }
    }

    private void UpdateCurrencyUI()
    {
        if (GoldenCoin != null) GoldenCoin.text = gold.ToString();
        if (coperCoin != null) coperCoin.text = coper.ToString();
        if (SilverCOin != null) SilverCOin.text = silver.ToString();
    }

    public void CheckItemDetailsById(int itemId)
    {
        TowarInfoItems towarInfo = GetItemInfoById(itemId);
        if (towarInfo != null)
        {
            Debug.Log($"ID: {towarInfo.itemId} | Name: {towarInfo.itemName} | Str: {towarInfo.BuystrengthText} | Arm: {towarInfo.BuyArmour}");
        }
    }

    public TowarInfoItems GetItemInfoById(int itemId)
    {
        if (towarButtons == null) return null;

        foreach (GameObject button in towarButtons)
        {
            if (button == null) continue;
            TowarInfoItems towarInfo = button.GetComponent<TowarInfoItems>();
            if (towarInfo != null && towarInfo.itemId == itemId)
            {
                return towarInfo;
            }
        }
        return null;
    }

    public void SpawnPrefabID2()
    {
        if (prefabToSpawn2 != null && player != null)
        {
            Vector3 spawnPosition = player.position + new Vector3(0, spawnOffset, 0);
            Instantiate(prefabToSpawn2, spawnPosition, Quaternion.identity);
        }
    }
}
