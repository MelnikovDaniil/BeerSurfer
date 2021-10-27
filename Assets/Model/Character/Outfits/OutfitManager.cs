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

    private List<OutfitModel> allOutfits;

    private void Awake()
    {
        SetUpOutfits();
    }

    public void SetUpOutfits()
    {
        allOutfits = new List<OutfitModel>
        {
            new OutfitModel { OutfitType = OutfitType.Head, Renderer = characterHead, Sprites = outfitLibrary.headSprites },
            new OutfitModel { OutfitType = OutfitType.Glasses, Renderer = characterGlasses, Sprites = outfitLibrary.glassesSprites },
            new OutfitModel { OutfitType = OutfitType.Torso, Renderer = characterTorso, Sprites = outfitLibrary.torsoSprites },
            new OutfitModel { OutfitType = OutfitType.Legs, Renderer = characterLegs, Sprites = outfitLibrary.legsSprites },
            new OutfitModel { OutfitType = OutfitType.Socks, Renderer = characterSocks, Sprites = outfitLibrary.socksSprites },
            new OutfitModel { OutfitType = OutfitType.Boots, Renderer = characterBoots, Sprites = outfitLibrary.bootsSprites },
        };
    }

    public void ResetOutfits()
    {
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

    public string GenerateRandomOutfitKit()
    {
        var suitableKit = false;
        var kitName = string.Empty;

        while(!suitableKit)
        {
            kitName = outfitLibrary.torsoSprites.Where(x => !x.name.Contains("Default")).GetRandom().name.Split('_')[0];
            suitableKit = allOutfits.All(typeOutfit => typeOutfit.Sprites.Any(sprite => sprite.name.Contains(kitName)));
        }

        foreach (var typeOutfit in allOutfits)
        {
            var sprites = allOutfits.First(x => x.OutfitType == typeOutfit.OutfitType).Sprites;
            var outfitSprite = sprites.First(x => x.name.Contains(kitName));

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

        return kitName;
    }

    public void SetUpKit(string kitName, int runCount)
    {
        foreach (var typeOutfit in allOutfits)
        {
            var outfitName = OutfitMapper.GetOutfit(typeOutfit.OutfitType);
            var newOutfit = typeOutfit.Sprites.First(x => x.name.Contains(kitName));

            if (!OutfitMapper.IsOutfitAvailable(newOutfit.name))
            {
                OutfitMapper.SetTempraryOutfit(newOutfit.name);
                OutfitMapper.SetRememberOutfit(outfitName);
            }

            OutfitMapper.SetOutfit(typeOutfit.OutfitType, newOutfit);
        }

        OutfitMapper.SetTempraryDuration(runCount);
    }

    public void RemoveTemproryKit()
    {
        foreach (var typeOutfit in allOutfits)
        {
            var currentOutfitName = OutfitMapper.GetOutfit(typeOutfit.OutfitType);
            var previousOutfit = typeOutfit.Sprites.FirstOrDefault(x => OutfitMapper.GetOutfitStatus(x.name) == OutfitMapper.OutfitStatus.Previous);
            var temproryOutfit = typeOutfit.Sprites.FirstOrDefault(x => OutfitMapper.GetOutfitStatus(x.name) == OutfitMapper.OutfitStatus.Temporary);
            if (temproryOutfit != null)
            {
                if (currentOutfitName == temproryOutfit.name)
                {
                    OutfitMapper.SetOutfit(typeOutfit.OutfitType, previousOutfit);
                }

                OutfitMapper.SetAvailableOutfit(previousOutfit.name);
                OutfitMapper.SetUnAvailableOutfit(temproryOutfit.name);
            }
            
        }
    }
}
