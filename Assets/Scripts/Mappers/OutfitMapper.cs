using UnityEngine;

public static class OutfitMapper
{
    private const string MapperName = "Outfit";

    public static bool IsOutfitAvailable(string name)
    {
        if (name.Contains("Default"))
        {
            return true;
        }

        var outfitStatus = PlayerPrefs.GetInt(MapperName + name);
        return outfitStatus == 1; 
    }

    public static void SetAvailableOutfit(string name)
    {
        PlayerPrefs.SetInt(MapperName + name, 1);
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
