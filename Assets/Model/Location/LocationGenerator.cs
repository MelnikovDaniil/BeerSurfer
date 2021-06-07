using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LocationGenerator : MonoBehaviour
{
    public const int BgOrderingLayer1 = -11;
    public const int BgOrderingLayer2 = -12;

    public float paralaxOffset = 645;
    public float paralaxCloudsSpeed = 4;
    public float paralaxAdditionalBgSpeed = 4;
    public float paralaxGroundSpeed = 10;

    [Space(20)]
    public List<ScriptableLocation> locations;
    public int locationLenght = 10;

    [Space(20)]
    public List<SpriteRenderer> paralaxFirstLayerBg;
    public List<SpriteRenderer> paralaxSecondLayerBg;
    public List<RoadPart> paralaxGround;
    public List<Transform> obstacles;
    public Transform obstacleContainer;

    [Space(20)]
    public int obstaclesCount;
    public MinMaxCurve minMaxObstclesDistance = new MinMaxCurve(120, 250);
    public List<RectTransform> obstaclesPrefabs;
    public RectTransform finishPrefab;

    [Space(20)]
    public BeerView beerPrefab;
    public List<Sprite> beerSprites;
    public float beerGap = 1;
    public float beeerSpawnHeight = -3.3f;
    public MinMaxCurve minMaxBeerOnScreen = new MinMaxCurve(0, 4);

    private Queue<Sprite> roadQueue;

    private int currentBgOrdering;

    private void Start()
    {
        currentBgOrdering = BgOrderingLayer1;
        roadQueue = new Queue<Sprite>();
    }

    private void Update()
    {
        ParalaxMove(paralaxFirstLayerBg, paralaxAdditionalBgSpeed);
        ParalaxMove(paralaxSecondLayerBg, paralaxAdditionalBgSpeed);
        GroundMove(paralaxGround, paralaxGroundSpeed);
        obstacles.ForEach(x => x.position += Vector3.left * paralaxGroundSpeed * Time.deltaTime);
    }

    public void ParalaxMove(IEnumerable<SpriteRenderer> paralaxItems, float speed)
    {
        var vectorSpeed = Vector3.left * speed * Time.deltaTime;
        var itemsCount = paralaxItems.Count();
        var itemMoveDistance = paralaxOffset * itemsCount / (itemsCount - 1);
        foreach (var item in paralaxItems)
        {
            if (item.transform.position.x < -itemMoveDistance)
            {
                item.transform.position += new Vector3(paralaxOffset * itemsCount, 0);
            }
            item.transform.position += vectorSpeed;
        }
    }

    public void GroundMove(IEnumerable<RoadPart> paralaxItems, float speed)
    {
        var vectorSpeed = Vector3.left * speed * Time.deltaTime;
        var itemsCount = paralaxItems.Count();
        var itemMoveDistance = paralaxOffset * itemsCount / (itemsCount - 1);
        foreach (var road in paralaxItems)
        {
            if (road.transform.position.x < -itemMoveDistance)
            {
                road.Clear();

                var roadType = roadQueue.Count == 1 ? RoadType.Finish : RoadType.Middle;
                road.transform.position += new Vector3(paralaxOffset * itemsCount, 0);

                if (!roadQueue.Any())
                {
                    roadType = RoadType.Start;
                    GenerateLocation();
                }

                road.ChangeSprite(roadQueue.Dequeue());
                road.SetMaskOrder(currentBgOrdering);
                road.roadType = roadType;
                GenerateBeer(road);
            }
            road.transform.position += vectorSpeed;
        }
    }

    //private void GenerateObstacles()
    //{
    //    var x = 500f;
    //    foreach (Transform obctacle in obstacleContainer)
    //    {
    //        Destroy(obctacle.gameObject);
    //    }
    //    obstacleContainer.anchoredPosition = new Vector2(0, obstacleContainer.anchoredPosition.y);

    //    for (var i = 0; i < obstaclesCount; i++)
    //    {
    //        var randomDistance = Random.Range(minMaxObstclesDistance.constantMin, minMaxObstclesDistance.constantMax);
    //        x = CreateObstacle(x);
    //        x += randomDistance;
    //    }
    //    var finish = Instantiate(finishPrefab, obstacleContainer);
    //    finish.anchoredPosition = new Vector3(x, 0);
    //    finish.pivot = new Vector2(0.5f, 0);
    //}

    private float CreateObstacle(float x)
    {
        var randomObstaclesCount = Random.Range(0, obstaclesPrefabs.Count);
        for (var i = 0; i < randomObstaclesCount; i++)
        {
            var obst = Instantiate(obstaclesPrefabs.GetRandom(), obstacleContainer);
            obst.anchoredPosition = new Vector3(x, 0);
            obst.pivot = new Vector2(0.5f, 0);
            x += 35;
        }

        return x;
    }

    private void GenerateLocation()
    {
        var location = locations.GetRandom();
        if (currentBgOrdering == BgOrderingLayer1)
        {
            currentBgOrdering = BgOrderingLayer2;
            paralaxSecondLayerBg.ForEach(x => x.sprite = location.Background);
        }
        else
        {
            currentBgOrdering = BgOrderingLayer1;
            paralaxFirstLayerBg.ForEach(x => x.sprite = location.Background);
        }

        roadQueue.Enqueue(location.startSprite);
        
        for (var i = 0; i < locationLenght; i++)
        {
            roadQueue.Enqueue(location.middleSprites.GetRandom());
        }
        roadQueue.Enqueue(location.finishSprite);
    }

    private void GenerateBeer(RoadPart roadPart)
    {
        var beerSprite = beerSprites.GetRandom();
        var beerCount = Random.Range(minMaxBeerOnScreen.constantMin, minMaxBeerOnScreen.constantMax);
        var x = (beerCount - 1) / 2 * -beerGap;
        for (var i = 0; i < beerCount; i++)
        {
            var spawnedBeer = Instantiate(beerPrefab, roadPart.transform);
            spawnedBeer.transform.localPosition = new Vector3(x, beeerSpawnHeight);
            spawnedBeer.ChangeSprite(beerSprite);
            roadPart.beerList.Add(spawnedBeer);
            x += beerGap;
        }
    }
}
