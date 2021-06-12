using System;
using UnityEngine;

public enum ObstacleType
{
    Bottom,
    Middle,
    Top,
}

[Serializable]
public class ObstacleHigh
{
    public ObstacleType type;
    public float high;
}