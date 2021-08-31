using System;
using UnityEngine;

[Serializable]
public class GenerationCriteria
{
    public float raceTime = 30f;

    public ScriptableLocation location;

    [Space(20f)]
    [Header("Bonuses")]
    public bool batSpawn = false;

    public bool chillySpawn = false;

    [Space(20f)]
    [Header("Obstacles")]
    public bool enableJumpObstacles = true;

    public bool enableSlipObstacles = true;

    public bool enableMixedObstacles = true;

    [Range(0f, 1f)]
    public float obstacleChanse = 0.75f; 
}
