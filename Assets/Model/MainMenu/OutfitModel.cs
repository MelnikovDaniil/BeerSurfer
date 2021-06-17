using System;
using System.Collections.Generic;
using UnityEngine;

public class OutfitModel : ICloneable
{
    public List<Sprite> Sprites { get; set; }
    public SpriteRenderer Renderer { get; set; }
    public OutfitType OutfitType { get; set; }

    public object Clone()
    {
        var outfitModel = new OutfitModel
        {
            Sprites = new List<Sprite>(),
            Renderer = Renderer,
            OutfitType = OutfitType,
        };

        Sprites.ForEach(x => outfitModel.Sprites.Add(x));

        return outfitModel;
    }
}
