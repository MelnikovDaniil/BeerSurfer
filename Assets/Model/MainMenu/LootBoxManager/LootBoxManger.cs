using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LootBoxManger : MonoBehaviour
{
    public OutfitLibrary outfitLibrary;
    public Image boxOutfit;
    public GameObject notification;
    public Text notificationText;
    public GameObject returnShopButton;

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
        var lootBoxCount = LootBoxMapper.Get();
        notificationText.text = lootBoxCount.ToString();
        returnShopButton.SetActive(true);
        if (lootBoxCount > 0)
        {
            _animator.SetTrigger("drop");
        }
    }

    public void HideMenu()
    {
        _animator.SetTrigger("hide");
        returnShopButton.SetActive(false);
    }

    public void Refresh()
    {
        _animator.SetTrigger("hide");
        var lootBoxCount = LootBoxMapper.Get();
        notificationText.text = lootBoxCount.ToString();
        returnShopButton.SetActive(true);
        if (lootBoxCount > 0)
        {
            _animator.SetTrigger("drop");
        }
        UpdateNotification();
    }

    public void OpenLootBox()
    {
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
