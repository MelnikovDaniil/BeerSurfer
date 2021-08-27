using UnityEngine;

public static class GuideMapper
{
    private const string MapperName = "Guide";

    public static bool IsActive()
    {
        return PlayerPrefs.GetInt(MapperName + "Active", 1) == 1;
    }

    public static void SetActive(bool bl)
    {
        PlayerPrefs.SetInt(MapperName + "Active", bl ? 1 : 0);
    }

    public static int GetStep()
    {
        return PlayerPrefs.GetInt(MapperName + "Step", 0);
    }

    public static void SetStep(int step)
    {
        PlayerPrefs.SetInt(MapperName + "Step", step);
    }
}
