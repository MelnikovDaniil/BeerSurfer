using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Location")]
public class ScriptableLocation : ScriptableObject
{
    public Sprite startSprite;
    public List<Sprite> middleSprites;
    public Sprite finishSprite;
    public Sprite Background;
}
