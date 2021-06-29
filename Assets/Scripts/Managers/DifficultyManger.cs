using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManger : MonoBehaviour
{
    public LocationGenerator locationGenerator;
    public int speedChangingScore = 15000;
    public float speedDifference = 3f;
    public float difficltUpdateTime = 2f;

    private float additionalBgSpeed;
    private float groundSpeed;

    private float differenceCoof;

    private void Start()
    {
        additionalBgSpeed = locationGenerator.paralaxAdditionalBgSpeed;
        groundSpeed = locationGenerator.paralaxGroundSpeed;
        InvokeRepeating(nameof(ChangeDifference), 0, difficltUpdateTime);
    }

    public void ChangeDifference()
    {
        differenceCoof = (float)GameManager.score / speedChangingScore * (speedDifference - 1);
        locationGenerator.paralaxAdditionalBgSpeed = additionalBgSpeed * (differenceCoof + 1);
        locationGenerator.paralaxGroundSpeed = groundSpeed * (differenceCoof + 1);
    }
}
