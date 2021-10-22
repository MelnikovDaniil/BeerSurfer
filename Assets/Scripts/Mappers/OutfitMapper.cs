using UnityEngine;

public static class OutfitMapper
{
    private enum OutfitStatus
    {
        UnAvailable = 0,
        Available = 1,
        Previous = 2,
        Temporary = 3,
    }
    private const string MapperName = "Outfit";

    public static bool IsOutfitAvailable(string name)
    {
        if (name.Contains("Default"))
        {
            return true;
        }

        var outfitStatus = PlayerPrefs.GetInt(MapperName + name);
        return outfitStatus > 0; 
    }

    public static void SetUnAvailableOutfit(string name)
    {
        PlayerPrefs.SetInt(MapperName + name, (int)OutfitStatus.UnAvailable);
    }

    public static void SetAvailableOutfit(string name)
    {
        PlayerPrefs.SetInt(MapperName + name, (int)OutfitStatus.Available);
    }

    public static void SetRemenberOutfit(string name)
    {
        PlayerPrefs.SetInt(MapperName + name, (int)OutfitStatus.Previous);
    }

    public static void SetTemporaryOutfit(string name)
    {
        PlayerPrefs.SetInt(MapperName + name, (int)OutfitStatus.Temporary);
    }

    public static void SetOutfit(OutfitType type, Sprite sprite)
    {
        PlayerPrefs.SetString(MapperName + type, sprite.name);
    }

    public static string GetOutfit(OutfitType type)
    {
        return PlayerPrefs.GetString(MapperName + type);
    }
}
