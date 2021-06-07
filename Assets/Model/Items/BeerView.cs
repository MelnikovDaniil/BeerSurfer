using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerView : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }
}
