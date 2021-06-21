using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Location")]
public class ScriptableLocation : ScriptableObject
{
    public LocationType locationType;

    [Space(20)]
    public Sprite startSprite;
    public List<Sprite> middleSprites;
    public Sprite finishSprite;

    [Space(20)]
    public Sprite startFrontSprite;
    public List<Sprite> middleFrontSprites;
    public Sprite finishFrontSprite;

    [Space(20)]
    public List<Sprite> Background;

    [Space(20)]
    public List<Obstacle> obstacles;
}
