using UnityEngine;

public class LootBoxMapper
{
    private const string MapperName = "LootBox";

    public static void AddOne()
    {
        var count = PlayerPrefs.GetInt(MapperName, 0) + 1;
        PlayerPrefs.SetInt(MapperName, count);
    }

    public static bool RemoveOne()
    {
        var boxCount = PlayerPrefs.GetInt(MapperName, 0);
        if (boxCount > 0)
        {
            boxCount -= 1;
            PlayerPrefs.SetInt(MapperName, boxCount);
        }

        return false;
    }

    public static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 0);
    }
}
