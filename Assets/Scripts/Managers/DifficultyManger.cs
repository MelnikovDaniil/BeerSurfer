using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManger : MonoBehaviour
{
    public static event Action<float> OnDifficultyChange;
    public static float differenceCoof;

    public LocationGenerator locationGenerator;
    public int maxScoreDifficulty = 8500;
    public float difficltUpdateTime = 2f;

    private void Awake()
    {
        OnDifficultyChange = null;
    }

    private void Start()
    {
        InvokeRepeating(nameof(ChangeDifference), 0, difficltUpdateTime);
    }

    public void ChangeDifference()
    {
        differenceCoof = Mathf.Clamp01((float)GameManager.score / maxScoreDifficulty);
        OnDifficultyChange?.Invoke(differenceCoof);
    }
}
