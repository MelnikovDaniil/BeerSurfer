using UnityEngine;

public static class RecordMapper
{
    private const string MapperName = "Record";

    public static void Set(int score)
    {
        PlayerPrefs.SetInt(MapperName, score);
    }

    public static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 0);
    }
}
