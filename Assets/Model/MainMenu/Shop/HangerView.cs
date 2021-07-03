using System;
using UnityEngine;

public class HangerView : MonoBehaviour
{
    public event Action OnHangerClickEvent;
    private void OnMouseDown()
    {
        OnHangerClickEvent?.Invoke();
    }
}
