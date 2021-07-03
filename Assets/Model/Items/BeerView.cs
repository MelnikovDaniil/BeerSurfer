using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerView : MonoBehaviour
{
    public ParticleSystem particles;

    private Animator _animator;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public Sprite GetSprite()
    {
        return _spriteRenderer.sprite;
    }

    public void Collect()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        particles.Play();
    }

    public void Disable()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
    }
}
