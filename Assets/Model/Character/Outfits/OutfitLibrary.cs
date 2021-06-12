using System.Collections.Generic;
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
}
