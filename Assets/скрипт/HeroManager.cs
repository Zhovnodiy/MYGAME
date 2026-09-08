using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public List<HeroSlot> heroSlots = new List<HeroSlot>();
    public Sprite testHeroIcon; // Додайте поле для іконки героя

    void Start()
    {
        LoadHeroSlots();
    }

    void LoadHeroSlots()
    {
        List<HeroData> heroes = new List<HeroData>
        {
            new HeroData { heroName = "Test 1", heroIcon = testHeroIcon, heroLevel = 13 },
            new HeroData { heroName = "Test 2", heroIcon = testHeroIcon, heroLevel = 10 }
            // Додайте більше героїв за потреби
        };

        for (int i = 0; i < heroes.Count && i < heroSlots.Count; i++)
        {
            heroSlots[i].SetHeroData(heroes[i]);
        }
    }
}
