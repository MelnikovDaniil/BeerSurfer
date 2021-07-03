using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxManger : MonoBehaviour
{
    public OutfitLibrary outfitLibrary;
    public MainMenu mainMenu;
    public Image boxOutfit;
    public GameObject notification;
    public Text notificationText;
    public GameObject returnShopButton;

    public Button byeLootboxButton;
    public Button openLootBoxButton;

    public int lootboxCost = 300;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        UpdateNotification();
    }

    public void UpdateNotification()
    {
        var lootBoxCount = LootBoxMapper.Get();
        notificationText.text = lootBoxCount.ToString();
        if (lootBoxCount <= 0)
        {
            notification.SetActive(false);
        }
    }

    public void OpenMenu()
    {
        openLootBoxButton.enabled = false;
        var lootBoxCount = LootBoxMapper.Get();
        notificationText.text = lootBoxCount.ToString();
        returnShopButton.SetActive(true);
        byeLootboxButton.gameObject.SetActive(false);
        if (lootBoxCount > 0)
        {
            openLootBoxButton.enabled = true;
            _animator.SetTrigger("drop");
        }
        else if (BeerMapper.Get() >= lootboxCost)
        {
            byeLootboxButton.gameObject.SetActive(true);
        }
    }

    public void HideMenu()
    {
        openLootBoxButton.enabled = false;
        var lootBoxCount = LootBoxMapper.Get();
        if (lootBoxCount > 0)
        {
            _animator.SetTrigger("hide");
        }
        returnShopButton.SetActive(false);
        byeLootboxButton.gameObject.SetActive(false);
    }

    public void Refresh()
    {
        openLootBoxButton.enabled = false;
        _animator.SetTrigger("hide");
        var lootBoxCount = LootBoxMapper.Get();
        notificationText.text = lootBoxCount.ToString();
        returnShopButton.SetActive(true);
        byeLootboxButton.gameObject.SetActive(false);
        if (lootBoxCount > 0)
        {
            openLootBoxButton.enabled = true;
            _animator.SetTrigger("drop");
        }
        else if (BeerMapper.Get() >= lootboxCost)
        {
            byeLootboxButton.gameObject.SetActive(true);
        }
        UpdateNotification();
    }

    public void ByeLootBox()
    {
        BeerMapper.Add(-lootboxCost);
        LootBoxMapper.AddOne();
        mainMenu.UpdateInfo();
        openLootBoxButton.enabled = true;
        _animator.SetTrigger("drop");
        UpdateNotification();
        byeLootboxButton.gameObject.SetActive(false);
    }

    public void OpenLootBox()
    {
        openLootBoxButton.enabled = false;
        returnShopButton.SetActive(false);
        _animator.SetTrigger("show");
        var outfit = GenerateRandomOutfit();
        boxOutfit.sprite = outfit;
        OutfitMapper.SetAvailableOutfit(outfit.name);
        LootBoxMapper.RemoveOne();
    }

    private Sprite GenerateRandomOutfit()
    {
        var sprites = outfitLibrary.headSprites
            .Concat(outfitLibrary.glassesSprites)
            .Concat(outfitLibrary.torsoSprites)
            .Concat(outfitLibrary.legsSprites)
            .Concat(outfitLibrary.socksSprites)
            .Concat(outfitLibrary.bootsSprites);

        sprites = sprites.Where(x => !OutfitMapper.IsOutfitAvailable(x.name));
        return sprites.GetRandom();
    }

}
