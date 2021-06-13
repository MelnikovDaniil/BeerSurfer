using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutfitManager : MonoBehaviour
{
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

    public void Start()
    {
        var allOutfits = new List<OutfitModel>
        {
            new OutfitModel { OutfitType = OutfitType.Head, Renderer = characterHead, Sprites = outfitLibrary.headSprites },
            new OutfitModel { OutfitType = OutfitType.Glasses, Renderer = characterGlasses, Sprites = outfitLibrary.glassesSprites },
            new OutfitModel { OutfitType = OutfitType.Torso, Renderer = characterTorso, Sprites = outfitLibrary.torsoSprites },
            new OutfitModel { OutfitType = OutfitType.Legs, Renderer = characterLegs, Sprites = outfitLibrary.legsSprites },
            new OutfitModel { OutfitType = OutfitType.Socks, Renderer = characterSocks, Sprites = outfitLibrary.socksSprites },
            new OutfitModel { OutfitType = OutfitType.Boots, Renderer = characterBoots, Sprites = outfitLibrary.bootsSprites },
        };

        foreach (var typeOutfit in allOutfits)
        {
            var spriteName = OutfitMapper.GetOutfit(typeOutfit.OutfitType);
            var sprites = allOutfits.First(x => x.OutfitType == typeOutfit.OutfitType).Sprites;
            var outfitSprite = !string.IsNullOrEmpty(spriteName) ? sprites.First(x => x.name == spriteName) : sprites.First(x => x.name.Contains("Default"));


            OutfitMapper.SetOutfit(typeOutfit.OutfitType, outfitSprite);
            typeOutfit.Renderer.sprite = outfitSprite;

            if (typeOutfit.OutfitType == OutfitType.Torso)
            {
                var outfitName = outfitSprite.name.Split('_').FirstOrDefault();
                if (outfitName != null)
                {
                    characterBackHand.sprite = outfitLibrary.backHandSprites.FirstOrDefault(x => x.name.Contains(outfitName));
                }
                else
                {
                    Debug.LogError($"Back hand for {outfitSprite.name} not found");
                }
            }
        }
    }
}
