using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    public ObstacleType obstacleType;

    [SerializeField]
    private List<BeerView> beerList;

    [SerializeField]
    [Range(0, 1)]
    private float everySecondBeerChance = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    private float allBeerChance = 0.15f;

    public void SetUpBeer()
    {
        beerList.ForEach(x => x.gameObject.SetActive(false));
        var chanse = Random.value;
        if (chanse < everySecondBeerChance)
        {
            EnableEverySecondBeer();
        }
        else if (chanse < allBeerChance)
        {
            EnableAllBeer();
        }

    }

    private void EnableAllBeer()
    {
        beerList.ForEach(x => x.gameObject.SetActive(true));
    }

    private void EnableEverySecondBeer()
    {
        for (int i = 0; i < beerList.Count-1; i += 2)
        {
            beerList[i].gameObject.SetActive(true);
        }
    }
}
