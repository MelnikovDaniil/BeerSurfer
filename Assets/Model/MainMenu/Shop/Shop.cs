using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public bool allOutfits;

    [Space(20)]
    public OutfitLibrary outfitLibrary;

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
    public Button equipButton;

    public DummyView previousDummy;
    public DummyView nextDummy;

    public Animator shopCharacterAnimator;
    [Range(0f, 1f)]
    public float inspectationChanse = 0.7f;

    private Animator _animator;
    private List<OutfitModel> shop;
    private List<OutfitModel> availableOutfits;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        shop = new List<OutfitModel>
        {
            new OutfitModel { OutfitType = OutfitType.Head, Renderer = characterHead, Sprites = outfitLibrary.headSprites },
            new OutfitModel { OutfitType = OutfitType.Glasses, Renderer = characterGlasses, Sprites = outfitLibrary.glassesSprites },
            new OutfitModel { OutfitType = OutfitType.Torso, Renderer = characterTorso, Sprites = outfitLibrary.torsoSprites },
            new OutfitModel { OutfitType = OutfitType.Legs, Renderer = characterLegs, Sprites = outfitLibrary.legsSprites },
            new OutfitModel { OutfitType = OutfitType.Socks, Renderer = characterSocks, Sprites = outfitLibrary.socksSprites },
            new OutfitModel { OutfitType = OutfitType.Boots, Renderer = characterBoots, Sprites = outfitLibrary.bootsSprites },
        };
    }

    private void Start()
    {
        if (allOutfits)
        {
            SetAllAvailable();
        }

        availableOutfits = GetAvailableOutfits();
        availableOutfits.Reverse();

        foreach (var outfit in availableOutfits)
        {
            ShowOutfitCategory(outfit.OutfitType);
        }
    }

    public void OpenMenu()
    {
        _animator.SetTrigger("show");
        availableOutfits = GetAvailableOutfits();
    }

    public void HideMenu()
    {
        _animator.SetTrigger("hide");
    }

    public void ShowOutfitCategory(OutfitTypeComponent typeComponent)
    {
        ShowOutfitCategory(typeComponent.outfitType);
    }

    private void SetUpEquipButton(OutfitType type, Sprite sprite)
    {
        var outfitName = OutfitMapper.GetOutfit(type);

        equipButton.onClick.RemoveAllListeners();
        equipButton.interactable = false;

        if (outfitName != sprite.name)
        {
            equipButton.interactable = true;
            equipButton.onClick.AddListener(() =>
            {
                OutfitMapper.SetOutfit(type, sprite);
                ShowOutfit(type, sprite);
            });
        }
    }

    private void ShowOutfitCategory(OutfitType type)
    {
        var spriteName = OutfitMapper.GetOutfit(type);
        var typeOutfits = availableOutfits.First(x => x.OutfitType == type).Sprites;
        var outfitSprite = !string.IsNullOrEmpty(spriteName) ? typeOutfits.First(x => x.name == spriteName) : typeOutfits.First(x => x.name.Contains("Default"));

        OutfitMapper.SetOutfit(type, outfitSprite);

        ShowOutfit(type, outfitSprite);
    }

    private void ShowOutfit(OutfitType type, Sprite sprite)
    {
        if (Random.value <= inspectationChanse)
        {
            shopCharacterAnimator.SetTrigger("inspect");
        }
        SoundManager.PlaySound("button");
        var typeOutfit = availableOutfits.First(x => x.OutfitType == type);
        var index = typeOutfit.Sprites.IndexOf(sprite);

        typeOutfit.Renderer.sprite = sprite;

        var previousOutfit = typeOutfit.Sprites.ElementAtOrDefault(index - 1);
        var nextOutfit = typeOutfit.Sprites.ElementAtOrDefault(index + 1);

        OutfitMapper.SetOutfit(type, sprite);

        ShowAdditionalOutfit(nextOutfit, nextButton, nextDummy, type);
        ShowAdditionalOutfit(previousOutfit, previousButton, previousDummy, type);

        if (type == OutfitType.Torso)
        {
            characterBackHand.sprite = outfitLibrary.GetBackHand(sprite);
        }

    }

    private void ShowAdditionalOutfit(Sprite outfit, Button button, DummyView dummyView, OutfitType type)
    {
        if (outfit != null)
        {
            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ShowOutfit(type, outfit));

            dummyView.gameObject.SetActive(true);
            dummyView.characterBackHand.sprite = characterBackHand.sprite;
            dummyView.characterBoots.sprite = characterBoots.sprite;
            dummyView.characterGlasses.sprite = characterGlasses.sprite;
            dummyView.characterHead.sprite = characterHead.sprite;
            dummyView.characterLegs.sprite = characterLegs.sprite;
            dummyView.characterSocks.sprite = characterSocks.sprite;
            dummyView.characterTorso.sprite = characterTorso.sprite;

            if (type == OutfitType.Torso)
            {
                var backhandSprite = outfitLibrary.GetBackHand(outfit);
                dummyView.ShowOutfit(type, outfit, backhandSprite);
            }
            else
            {
                dummyView.ShowOutfit(type, outfit);
            }
        }
        else
        {
            button.gameObject.SetActive(false);
            dummyView.gameObject.SetActive(false);
        }
    }

    private List<OutfitModel> GetAvailableOutfits()
    {
        var outfits = new List<OutfitModel>(shop.Count);

        shop.ForEach((item) =>
        {
            outfits.Add((OutfitModel)item.Clone());
        });

        foreach (var outfit in outfits)
        {
            outfit.Sprites = outfit.Sprites.Where(x => OutfitMapper.IsOutfitAvailable(x.name)).ToList();
        }
        return outfits;
    }

    private void SetAllAvailable()
    {
        shop.ForEach(x => x.Sprites.ForEach(sp => OutfitMapper.SetAvailableOutfit(sp.name)));
    }
}
