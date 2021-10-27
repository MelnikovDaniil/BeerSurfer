using UnityEngine;

public static class OfferMapper
{
    private const string MapperName = "Offer";

    public static void SetNextOfferFrom(int fromRace)
    {
        PlayerPrefs.SetInt(MapperName + "RaceLeft", fromRace);
    }

    public static int OfferLeft()
    {
        return PlayerPrefs.GetInt(MapperName + "RaceLeft", 10);
    }
}
