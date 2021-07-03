using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class LocationGenerator : MonoBehaviour
{
    public const int BgOrderingLayer1 = -12;
    public const int BgOrderingLayer2 = -13;

    public const int InnerRoadOrderingLayer = -10;
    public const int OpenRoadOrderingLayer = -11;

    public float paralaxOffset = 645;
    public float paralaxAdditionalBgSpeed = 4;
    public float paralaxGroundSpeed = 10;

    public float speedDifferenceFactor = 2f;

    [Space(20)]
    public List<ScriptableLocation> locations;
    public int locationLenght = 10;

    [Space(20)]
    public List<SpriteRenderer> paralaxFirstLayerBg;
    public List<SpriteRenderer> paralaxSecondLayerBg;
    public List<RoadPart> paralaxGround;
    public List<SpriteRenderer> paralaxFront;

    [Space(20)]
    [Range(0, 1)]
    public float obstacleChance = 0.5f;
    public List<ObstacleHigh> obstacleHigh = new List<ObstacleHigh>
    {
        new ObstacleHigh{ type = ObstacleType.Bottom, high = -3.3f},
        new ObstacleHigh{ type = ObstacleType.Middle, high = 0},
        new ObstacleHigh{ type = ObstacleType.Top, high = 3.3f},
    };

    [Space(20)]
    public int lootboxStartScore;
    [Range(0f, 1f)]
    public float lootBoxStawnChange = 0.05f;
    public int maxLootBoxByGame = 1;
    public LootBoxItemView lootBoxPrefab;

    private ScriptableLocation currentLocation;
    private Queue<Sprite> roadQueue;
    private Queue<Sprite> frontQueue;

    private int currentBgOrdering;

    private bool gameStarted;

    private float additionalBgSpeed;
    private float groundSpeed;
    
    private void Start()
    {
        DifficultyManger.OnDifficultyChange += OnDifficultyChange;

        currentBgOrdering = BgOrderingLayer1;
        roadQueue = new Queue<Sprite>();
        frontQueue = new Queue<Sprite>();
        groundSpeed = paralaxGroundSpeed;
        additionalBgSpeed = paralaxAdditionalBgSpeed;
        paralaxFirstLayerBg.ForEach(x => x.sortingOrder = BgOrderingLayer1);
        paralaxSecondLayerBg.ForEach(x => x.sortingOrder = BgOrderingLayer2);
    }

    private void Update()
    {
        if (gameStarted)
        {
            ParalaxMove(paralaxFirstLayerBg, additionalBgSpeed);
            ParalaxMove(paralaxSecondLayerBg, additionalBgSpeed);
            GroundMove(paralaxGround, groundSpeed);
            FrontMove(paralaxFront, groundSpeed);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
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

    public void FrontMove(IEnumerable<SpriteRenderer> paralaxItems, float speed)
    {
        var vectorSpeed = Vector3.left * speed * Time.deltaTime;
        var itemsCount = paralaxItems.Count();
        var itemMoveDistance = paralaxOffset * itemsCount / (itemsCount - 1);
        foreach (var item in paralaxItems)
        {
            if (item.transform.position.x < -itemMoveDistance)
            {
                item.transform.position += new Vector3(paralaxOffset * itemsCount, 0);
                item.sprite = frontQueue.Dequeue();
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
                road.SetOrder(LocationType.Open == currentLocation.locationType ? OpenRoadOrderingLayer : InnerRoadOrderingLayer);
                road.SetMaskOrder(currentBgOrdering);
                road.roadType = roadType;
                var randomChanse = Random.value;
                if (GameManager.score > lootboxStartScore && maxLootBoxByGame > 0 && randomChanse < lootBoxStawnChange)
                {
                    maxLootBoxByGame--;
                    GenerateLootBox(road);
                }
                else if (randomChanse < obstacleChance)
                {
                    GenerateObstacle(road);
                }
                BeerManager.Instance.GenerateBeer(road);
            }
            road.transform.position += vectorSpeed;
        }
    }

    private void GenerateLocation()
    {
        if (currentLocation == null)
        {
            currentLocation = locations.Where(x => x.locationType != LocationType.Inner).GetRandom();
        }
        else
        {
            currentLocation = locations.Where(x => x.locationType != currentLocation?.locationType).GetRandom();
        }

        if (currentBgOrdering == BgOrderingLayer1)
        {
            currentBgOrdering = BgOrderingLayer2;
            paralaxSecondLayerBg.ForEach(x => x.sprite = currentLocation.Background.GetRandom());
        }
        else
        {
            currentBgOrdering = BgOrderingLayer1;
            paralaxFirstLayerBg.ForEach(x => x.sprite = currentLocation.Background.GetRandom());
        }

        roadQueue.Enqueue(currentLocation.startSprite);
        frontQueue.Enqueue(currentLocation.startFrontSprite);

        for (var i = 0; i < locationLenght; i++)
        {
            roadQueue.Enqueue(currentLocation.middleSprites.GetRandom());
            frontQueue.Enqueue(currentLocation.middleFrontSprites.GetRandomOrDefault());
        }
        roadQueue.Enqueue(currentLocation.finishSprite);
        frontQueue.Enqueue(currentLocation.finishFrontSprite);
    }

    private void GenerateLootBox(RoadPart roadPart)
    {
        var spawnedLootBox = Instantiate(lootBoxPrefab, roadPart.transform);
        spawnedLootBox.transform.localPosition = new Vector3(0, 1);
        roadPart.objectToRemove.Add(spawnedLootBox.gameObject);
    }

    private void GenerateObstacle(RoadPart roadPart)
    {
        var obstacle = currentLocation.obstacles.GetRandom();
        var spawnedObstacle = Instantiate(obstacle, roadPart.transform);
        var high = obstacleHigh.First(x => x.type == obstacle.obstacleType).high;
        spawnedObstacle.transform.localPosition = new Vector3(0, high);

        roadPart.obstacle = spawnedObstacle;
        roadPart.objectToRemove.Add(spawnedObstacle.gameObject);
    }

    private void OnDifficultyChange(float difficultyCoof)
    {
        groundSpeed = paralaxGroundSpeed * difficultyCoof * speedDifferenceFactor + paralaxGroundSpeed;
        additionalBgSpeed = paralaxAdditionalBgSpeed * difficultyCoof * speedDifferenceFactor + paralaxAdditionalBgSpeed;
    }
}
