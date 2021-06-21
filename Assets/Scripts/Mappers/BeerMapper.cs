using UnityEngine;

public static class BeerMapper
{
    private const string MapperName = "Beer";

    public static void Add(int beerCount)
    {
        var count = PlayerPrefs.GetInt(MapperName, 0) + beerCount;
        PlayerPrefs.SetInt(MapperName, count);
    }

    public static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 0);
    }
}
