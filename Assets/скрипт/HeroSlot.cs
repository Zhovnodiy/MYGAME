using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class HeroData
{
    public string heroName;
    public Sprite heroIcon;
    public int heroLevel;
}

public class HeroSlot : MonoBehaviour
{
    public Image heroIconImage;
    public TMP_Text heroNameText;
    public TMP_Text heroLevelText;

    public void SetHeroData(HeroData heroData)
    {
        heroIconImage.sprite = heroData.heroIcon;
        heroNameText.text = heroData.heroName;
        heroLevelText.text = "Level: " + heroData.heroLevel.ToString();
    }
}