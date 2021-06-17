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
    [NonSerialized]
    public List<Obstacle> obstacles;
    [NonSerialized]
    public LootBoxItemView lootBox;

    private SpriteMask _spriteMask;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        beerList = new List<BeerView>();
        obstacles = new List<Obstacle>();
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

        foreach (var obstacle in obstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle.gameObject);
            }
        }

        if (lootBox != null)
        {
            Destroy(lootBox.gameObject);
            lootBox = null;
        }

        obstacles.Clear();
        beerList.Clear();
        
    }
}
