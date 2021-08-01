using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VideoOutfitChange : MonoBehaviour
{
    public float delay;
    public float outfitGap;

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

    private List<OutfitModel> outfitModels;

    private void Awake()
    {
        outfitModels = new List<OutfitModel>
        {
            new OutfitModel { OutfitType = OutfitType.Head, Renderer = characterHead, Sprites = outfitLibrary.headSprites },
            new OutfitModel { OutfitType = OutfitType.Glasses, Renderer = characterGlasses, Sprites = outfitLibrary.glassesSprites },
            new OutfitModel { OutfitType = OutfitType.Torso, Renderer = characterTorso, Sprites = outfitLibrary.torsoSprites },
            new OutfitModel { OutfitType = OutfitType.Legs, Renderer = characterLegs, Sprites = outfitLibrary.legsSprites },
            new OutfitModel { OutfitType = OutfitType.Socks, Renderer = characterSocks, Sprites = outfitLibrary.socksSprites },
            new OutfitModel { OutfitType = OutfitType.Boots, Renderer = characterBoots, Sprites = outfitLibrary.bootsSprites },
        };
        var animator = GetComponent<Animator>();
        animator.Play("Run");
    }

    private void Start()
    {
        InvokeRepeating(nameof(SetRandomSkin), delay, outfitGap);
    }

    private void SetRandomSkin()
    {
        foreach (var item in outfitModels)
        {
            var outfitSprite = item.Sprites.GetRandom();

            item.Renderer.sprite = outfitSprite;

            if (item.OutfitType == OutfitType.Torso)
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
