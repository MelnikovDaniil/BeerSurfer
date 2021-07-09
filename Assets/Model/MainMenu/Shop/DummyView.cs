using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DummyView : MonoBehaviour
{
    [Space(20)]
    public SpriteRenderer characterBackHand;
    public SpriteRenderer characterBoots;
    public SpriteRenderer characterGlasses;
    public SpriteRenderer characterHead;
    public SpriteRenderer characterLegs;
    public SpriteRenderer characterSocks;
    public SpriteRenderer characterTorso;

    [Range(0f, 1f)]
    [SerializeField]
    private float opacity = 0.5f;
    [Range(0f, 1f)]
    [SerializeField]
    private float previewOpacity = 0f;


    private List<OutfitModel> typeOutfits;

    void Awake()
    {
        SetUpOutfits();
    }

    private void SetUpOutfits()
    {
        typeOutfits = new List<OutfitModel>
        {
            new OutfitModel { OutfitType = OutfitType.Head, Renderer = characterHead },
            new OutfitModel { OutfitType = OutfitType.Glasses, Renderer = characterGlasses },
            new OutfitModel { OutfitType = OutfitType.Torso, Renderer = characterTorso },
            new OutfitModel { OutfitType = OutfitType.Legs, Renderer = characterLegs },
            new OutfitModel { OutfitType = OutfitType.Socks, Renderer = characterSocks },
            new OutfitModel { OutfitType = OutfitType.Boots, Renderer = characterBoots },
        };
    }


    public void ShowOutfit(OutfitType type, Sprite sprite, Sprite backHandSprite)
    {
        var color = new Color(1, 1, 1, opacity);
        ShowOutfit(type, sprite);
        characterBackHand.sprite = backHandSprite;
        characterBackHand.color = color;
    }

    public void ShowOutfit(OutfitType type, Sprite outfit)
    {
        var previewColor = new Color(1, 1, 1, previewOpacity);
        var color = new Color(1, 1, 1, opacity);
        characterBackHand.color = previewColor;
        SetUpOutfits();
        foreach (var typeOutfit in typeOutfits)
        {
            typeOutfit.Renderer.color = previewColor;
        }

        var outfitToChange = typeOutfits.First(x => x.OutfitType == type);
        outfitToChange.Renderer.sprite = outfit;
        outfitToChange.Renderer.color = color;
    }
}
