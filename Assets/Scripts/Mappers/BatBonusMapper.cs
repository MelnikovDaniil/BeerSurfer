using UnityEngine;

public static class BatBonusMapper
{
    private const string MapperName = "BatBonus";

    public static void AddOne()
    {
        var count = Get() + 1;
        PlayerPrefs.SetInt(MapperName, count);
    }

    public static bool RemoveOne()
    {
        var boxCount = Get();
        if (boxCount > 0)
        {
            boxCount -= 1;
            PlayerPrefs.SetInt(MapperName, boxCount);
        }

        return false;
    }

    public static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 3);
    }
}