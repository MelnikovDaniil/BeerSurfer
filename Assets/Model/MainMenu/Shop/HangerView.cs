using System;
using UnityEngine;

public class HangerView : MonoBehaviour
{
    public event Action OnHangerClickEvent;

    public SpriteRenderer tagRenderer;
    public Sprite workingShopTagSprite;

    private void OnMouseDown()
    {
        OnHangerClickEvent?.Invoke();
    }

    public void Enable()
    {
        tagRenderer.sprite = workingShopTagSprite;
    }

    public void Disable()
    {
        GetComponent<Animator>().enabled = false;
    }
}
