using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenerationCriteria
{
    public float raceTime = 30f;

    public List<ScriptableLocation> locations;
    public int innerLocationLength = 6;
    public int outerLocationLength = 6;

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
