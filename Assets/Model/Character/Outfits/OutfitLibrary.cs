using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Outfit/OutfitLibrary")]
public class OutfitLibrary : ScriptableObject
{
    public List<Sprite> backHandSprites;
    public List<Sprite> bootsSprites;
    public List<Sprite> glassesSprites;
    public List<Sprite> headSprites;
    public List<Sprite> legsSprites;
    public List<Sprite> socksSprites;
    public List<Sprite> torsoSprites;

    public Sprite GetBackHand(Sprite torsoSprite)
    {
        var outfitName = torsoSprite.name.Split('_').FirstOrDefault();
        if (outfitName != null)
        {
            return backHandSprites.FirstOrDefault(x => x.name.Contains(outfitName));
        }
        Debug.LogError($"Back hand for {torsoSprite.name} not found");
        return null;
    }
}
