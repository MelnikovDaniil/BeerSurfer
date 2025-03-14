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
    private Collider2D _startWallColliders;
    private Collider2D _finishWallColliders;
    private Collider2D _levelEndingColliders;

    private void Awake()
    {
        _spriteMask = GetComponent<SpriteMask>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        var colliders = GetComponents<BoxCollider2D>();
        _startWallColliders = colliders[0];
        _finishWallColliders = colliders[1];
        _levelEndingColliders = colliders[2];
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
        _startWallColliders.enabled = false;
        _finishWallColliders.enabled = false;
        _levelEndingColliders.enabled = false;
        objectToRemove.Clear();
    }

    public void EnableWalls()
    {
        switch (roadType)
        {
            case RoadType.Start:
                _startWallColliders.enabled = true;
                break;
            case RoadType.Finish:
                _finishWallColliders.enabled = true;
                break;
            case RoadType.LevelEnding:
                _levelEndingColliders.enabled = true;
                _levelEndingColliders.tag = "LevelEnding";
                break;
        }
    }
}
