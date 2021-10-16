using System;
using UnityEngine;

public static class LevelMapper
{
    private const string MapperName = "Level";

    public static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 1);
    }

    public static void Next()
    {
        var level = Get();
        PlayerPrefs.SetInt(MapperName, level + 1);
    }

    public static int GetAttempt()
    {
        return PlayerPrefs.GetInt(MapperName + "Attempt", 0);
    }
    public static void AddAttempt()
    {
        var attempt = GetAttempt();
        PlayerPrefs.SetInt(MapperName + "Attempt", attempt + 1);
    }

    public static void ResetAttempts()
    {
        var attempt = GetAttempt();
        PlayerPrefs.SetInt(MapperName + "Attempt", 0);
    }
}
