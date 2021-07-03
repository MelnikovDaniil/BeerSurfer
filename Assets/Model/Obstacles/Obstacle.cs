using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleType obstacleType;

    public List<BeerPatternType> beerPatterns = new List<BeerPatternType> 
    {
        BeerPatternType.Parabola,
    };

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var assetLibrary = AssetDatabase.LoadAssetAtPath<BeerPatternLibrary>("Assets/Model/Obstacles/BeerPatternLibrary.asset");
        Gizmos.color = Color.green;
        foreach (var beerPattern in beerPatterns)
        {
            var beerPosition = assetLibrary.beerPatterns.First(x => x.type == beerPattern).beerPositions;
            foreach (var position in beerPosition)
            {
                Gizmos.DrawWireSphere(position, 0.5f);
            }
            Gizmos.color = Gizmos.color.Next();
        }
    }
#endif
}
