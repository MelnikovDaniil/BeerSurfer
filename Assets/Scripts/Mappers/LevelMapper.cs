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
}
