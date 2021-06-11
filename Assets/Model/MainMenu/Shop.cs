using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public bool allOutfits;

    [Space(20)]
    public List<Sprite> backHandSprites;
    public List<Sprite> bootsSprites;
    public List<Sprite> glassesSprites;
    public List<Sprite> headSprites;
    public List<Sprite> legsSprites;
    public List<Sprite> socksSprites;
    public List<Sprite> torsoSprites;

    [Space(20)]
    public SpriteRenderer characterBackHand;
    public SpriteRenderer characterBoots;
    public SpriteRenderer characterGlasses;
    public SpriteRenderer characterHead;
    public SpriteRenderer characterLegs;
    public SpriteRenderer characterSocks;
    public SpriteRenderer characterTorso;

    [Space(20)]
    public Button previousButton;
    public Button nextButton;

    public Image previousOutfitImage;
    public Image nextOutfitImage;

    private List<OutfitModel> shop;
    private List<OutfitModel> availableOutfits;
    private void Awake()
    {
        shop = new List<OutfitModel>
        {
            new OutfitModel { OutfitType = OutfitType.Head, Renderer = characterHead, Sprites = headSprites },
            new OutfitModel { OutfitType = OutfitType.Glasses, Renderer = characterGlasses, Sprites = glassesSprites },
            new OutfitModel { OutfitType = OutfitType.Torso, Renderer = characterTorso, Sprites = torsoSprites },
            new OutfitModel { OutfitType = OutfitType.Legs, Renderer = characterLegs, Sprites = legsSprites },
            new OutfitModel { OutfitType = OutfitType.Socks, Renderer = characterSocks, Sprites = socksSprites },
            new OutfitModel { OutfitType = OutfitType.Boots, Renderer = characterBoots, Sprites = bootsSprites },
        };


        if (allOutfits)
            SetAllAvailable();

        availableOutfits = GetAvailableOutfits();
        availableOutfits.Reverse();

        foreach (var outfit in availableOutfits)
        {
            ShowOutfitCategory(outfit.OutfitType);
        }
    }

    public void ShowOutfitCategory(OutfitTypeComponent typeComponent)
    {
        ShowOutfitCategory(typeComponent.outfitType);

    }

    private void ShowOutfitCategory(OutfitType type)
    {
        var spriteName = OutfitMapper.GetOutfit(type);
        var typeOutfits = availableOutfits.First(x => x.OutfitType == type).Sprites;
        var outfitSprite = !string.IsNullOrEmpty(spriteName) ? typeOutfits.First(x => x.name == spriteName) : typeOutfits.First(x => x.name.Contains("Default"));
        ShowOutfit(type, outfitSprite);
    }

    private void ShowOutfit(OutfitType type, Sprite sprite)
    {
        var typeOutfit = availableOutfits.First(x => x.OutfitType == type);
        var index = typeOutfit.Sprites.IndexOf(sprite);

        typeOutfit.Renderer.sprite = sprite;

        var previousOutfit = typeOutfit.Sprites.ElementAtOrDefault(index - 1);
        var nextOutfit = typeOutfit.Sprites.ElementAtOrDefault(index + 1);

        ShowAdditionalOutfit(nextOutfit, nextButton, nextOutfitImage, type);
        ShowAdditionalOutfit(previousOutfit, previousButton, previousOutfitImage, type);
    }

    private void ShowAdditionalOutfit(Sprite outfit, Button button, Image image, OutfitType type)
    {

        if (outfit != null)
        {
            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ShowOutfit(type, outfit));

            image.gameObject.SetActive(true);
            image.sprite = outfit;
        }
        else
        {
            button.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
        }
    }

    private List<OutfitModel> GetAvailableOutfits()
    {
        var outfits = new List<OutfitModel>(shop);
        foreach (var outfit in outfits)
        {
            outfit.Sprites = outfit.Sprites.Where(x => OutfitMapper.IsOutfitAvailable(x.name)).ToList();
        }
        return outfits;
    }

    internal void SetAllAvailable()
    {
        shop.ForEach(x => x.Sprites.ForEach(sp => OutfitMapper.SetAvailableOutfit(sp.name)));
    }
}
