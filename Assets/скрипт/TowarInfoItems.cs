using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
public enum TypeMoney
{
    Gold,
    Coper,
    Silver
}
public enum ItemsTypes
{
    All,
    Weapon,
    Potion,
    Armour,
    Food
}
public class TowarInfoItems : MonoBehaviour
{
    
    [SerializeField]
    private TypeMoney moneyType;
    [SerializeField]
    private ItemsTypes ItemsTypes;
    public string itemName; // Назва товару
    public Sprite itemImage; // Зображення товару
    public int itemQuantity; // Кількість товару
    public float itemPrice; // Ціна товару

 public string BuyitemNameText; // Текст для назви предмета
   public int BuystrengthText; // Текст для сили
  public int BuyagilityText; // Текст для спритності
   public int BuyintelligenceText; // Текст для інтелекту
 public int BuyArmour;
 public int BuyQality;
  public int BuyFire_RES;
public int BuyColl_RES;
 public int BuyLight_RES;
  public string SetComplect;
    public int itemId ;

    public GameObject itemPrefab;

    public TypeMoney MoneyType => moneyType;
    public void DisplayItemInfo()
    {
        Debug.Log("Назва: " + itemName);
        Debug.Log("Зображення: " + itemImage); // Виводимо ім'я зображення
        Debug.Log("Кількість: " + itemQuantity);
        Debug.Log("Ціна: " + itemPrice);
        Debug.Log("качество: " + BuyQality);
        Debug.Log("броня: " + BuyArmour);
        
    }

}
