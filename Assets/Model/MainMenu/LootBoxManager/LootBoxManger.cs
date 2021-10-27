using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LootBoxManger : MonoBehaviour
{
    [Range(0, 1f)]
    public float outfitChanse = 0.6f;
    public OutfitLibrary outfitLibrary;
    public MainMenu mainMenu;
    public DummyView dummy;
    public Image bonusImage;
    public GameObject notification;
    public Text notificationText;
    public GameObject returnShopButton;

    public Button byeLootboxButton;
    public Button openLootBoxButton;

    public Sprite doubleBeerBonusSprite;
    public Sprite batSprite;

    public int lootboxCost = 300;

    [Space(20)]
    public Animator statisticPanel;
    public Text discountText;
    public Text batText;
    public Text beerText;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        UpdateNotification();
        UpdateInfo();
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
        UpdateInfo();
        statisticPanel.gameObject.SetActive(true);
        statisticPanel.SetTrigger("showHide");
        openLootBoxButton.enabled = false;
        var lootBoxCount = LootBoxMapper.Get();
        notificationText.text = lootBoxCount.ToString();
        returnShopButton.SetActive(true);
        byeLootboxButton.gameObject.SetActive(true);
        byeLootboxButton.interactable = false;
        if (lootBoxCount > 0)
        {
            openLootBoxButton.enabled = true;
            _animator.SetTrigger("drop");
        }
        else if (BeerMapper.Get() >= lootboxCost)
        {
            byeLootboxButton.interactable = true;
        }
    }

    public void HideMenu()
    {
        statisticPanel.SetTrigger("showHide");
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
        byeLootboxButton.gameObject.SetActive(true);
        byeLootboxButton.interactable = false;
        if (lootBoxCount > 0)
        {
            openLootBoxButton.enabled = true;
            _animator.SetTrigger("drop");
        }
        else if (BeerMapper.Get() >= lootboxCost)
        {
            byeLootboxButton.interactable = true;
        }
        UpdateNotification();
        UpdateInfo();
    }

    public void ByeLootBox()
    {
        BeerMapper.Add(-lootboxCost);
        LootBoxMapper.AddOne();
        mainMenu.UpdateInfo();
        openLootBoxButton.enabled = true;
        _animator.SetTrigger("drop");
        UpdateNotification();
        UpdateInfo();
        byeLootboxButton.gameObject.SetActive(false);
    }

    public void OpenLootBox()
    {
        openLootBoxButton.enabled = false;
        returnShopButton.SetActive(false);
        bonusImage.enabled = false;
        dummy.gameObject.SetActive(false);

        _animator.SetTrigger("show");
        var typeOutfit = GenerateRandomOutfit();
        var randomState = Random.state;
        Random.InitState((int)DateTime.Now.Ticks);
        if (Random.value < outfitChanse && typeOutfit.outfit != null)
        {
            dummy.gameObject.SetActive(true);
            if (typeOutfit.type == OutfitType.Torso)
            {
                var backHand = outfitLibrary.GetBackHand(typeOutfit.outfit);
                dummy.ShowOutfit(typeOutfit.type, typeOutfit.outfit, backHand);
            }
            else
            {
                dummy.ShowOutfit(typeOutfit.type, typeOutfit.outfit);
            }

            OutfitMapper.SetAvailableOutfit(typeOutfit.outfit.name);
        }
        else
        {
            bonusImage.enabled = true;

            if (Random.value < 0.5f)
            {
                bonusImage.sprite = batSprite;
                BatBonusMapper.AddOne();
            }
            else
            {
                bonusImage.sprite = doubleBeerBonusSprite;
                DobleBeerBonusMapper.AddOne();
            }
        }

        Random.state = randomState;
        LootBoxMapper.RemoveOne();
    }

    private (Sprite outfit, OutfitType type) GenerateRandomOutfit()
    {
        var typeOutfits = new List<(List<Sprite> sprites, OutfitType type)>
        {
            (outfitLibrary.headSprites, OutfitType.Head),
            (outfitLibrary.glassesSprites, OutfitType.Glasses),
            (outfitLibrary.torsoSprites, OutfitType.Torso),
            (outfitLibrary.legsSprites, OutfitType.Legs),
            (outfitLibrary.socksSprites, OutfitType.Socks),
            (outfitLibrary.bootsSprites, OutfitType.Boots),
        };

        var sprites = typeOutfits.SelectMany(x => x.sprites);

        sprites = sprites.Where(x => !OutfitMapper.IsOutfitAvailable(x.name));
        var outfit = sprites.GetRandomOrDefault();
        if (outfit == null)
        {
            return (null, default);
        }

        var type = typeOutfits.FirstOrDefault(x => x.sprites.Contains(outfit)).type;
        return (outfit, type);
    }

    public void UpdateInfo()
    {
        discountText.transform.parent.gameObject.SetActive(false);
        batText.transform.parent.gameObject.SetActive(false);
        beerText.text = BeerMapper.Get().ToString();

        var discountCount = DobleBeerBonusMapper.Get();
        if (discountCount > 0)
        {
            discountText.transform.parent.gameObject.SetActive(true);
            discountText.text = discountCount.ToString();
        }

        var batCount = BatBonusMapper.Get();
        if (batCount > 0)
        {
            batText.transform.parent.gameObject.SetActive(true);
            batText.text = batCount.ToString();
        }
    }
}
