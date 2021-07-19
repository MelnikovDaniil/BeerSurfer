using System;
using UnityEngine;

public class BonusView : MonoBehaviour
{
    public ScriptableBuff buffInitializator;

    public event Action OnBonusPickUp;

    public void PickUp()
    {
        OnBonusPickUp?.Invoke();
    }
}
