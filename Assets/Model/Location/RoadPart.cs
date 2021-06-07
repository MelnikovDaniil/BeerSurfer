using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadPart : MonoBehaviour
{
    [NonSerialized]
    public RoadType roadType;
    [NonSerialized]
    public List<BeerView> beerList;

    private SpriteMask _spriteMask;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        beerList = new List<BeerView>();
        _spriteMask = GetComponent<SpriteMask>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void SetMaskOrder(int orderingLayer)
    {
        _spriteMask.frontSortingOrder = orderingLayer;
    }

    public void Clear()
    {
        foreach (var beer in beerList)
        {
            if (beer != null)
            {
                Destroy(beer.gameObject);
            }
        }
        beerList.Clear();
    }
}
