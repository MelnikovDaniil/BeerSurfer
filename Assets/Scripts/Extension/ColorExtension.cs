using UnityEngine;
using Random = UnityEngine.Random;

public static class ColorExtension
{
    public static Color GetRandom()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    public static Color Next(this Color color)
    {
        Color.RGBToHSV(color, out var H, out var S, out float V);
        H = (H + 1f / 6f) % 1;
        return Color.HSVToRGB(H, S, V);
    }
}
