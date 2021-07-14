using UnityEngine;

public class SofaView : MonoBehaviour
{
    public Animator sleepingDrinker;
    public float drinkRepeatTime = 9;

    private void Start()
    {
        InvokeRepeating(nameof(SleepDrink), drinkRepeatTime, drinkRepeatTime);
    }

    private void SleepDrink()
    {
        sleepingDrinker.SetTrigger("drink");
    }
}
