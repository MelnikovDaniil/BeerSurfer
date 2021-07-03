using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadPart : MonoBehaviour
{
    [NonSerialized]
    public RoadType roadType;

    public List<GameObject> objectToRemove;
    public Obstacle obstacle;

    private SpriteMask _spriteMask;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteMask = GetComponent<SpriteMask>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void SetOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    public void SetMaskOrder(int orderingLayer)
    {
        _spriteMask.frontSortingOrder = orderingLayer;
    }

    public void Clear()
    {
        foreach (var obj in objectToRemove)
        {
            if (obj != null)
            {
                if (obj.TryGetComponent<BeerView>(out var beer))
                {
                    beer.Disable();
                }
                else
                {
                    Destroy(obj.gameObject);
                }
            }
        }
        obstacle = null;
        objectToRemove.Clear();
    }
}
