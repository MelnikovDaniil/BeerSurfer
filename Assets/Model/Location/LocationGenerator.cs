using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class LocationGenerator : MonoBehaviour
{
    public static LocationGenerator Instance;
    public static bool enableObstacleSpawn;
    public const int BgOrderingLayer1 = -12;
    public const int BgOrderingLayer2 = -13;

    public const int InnerRoadOrderingLayer = -10;
    public const int OpenRoadOrderingLayer = -11;

    public BeerPatternLibrary beerPatternLibrary;

    public float paralaxOffset = 645;
    public float paralaxAdditionalBgSpeed = 4;
    public float paralaxGroundSpeed = 10;

    public float speedDifferenceFactor = 2f;

    [Space(20)]
    public List<ScriptableLocation> locations;
    public int innerLocationLenght = 5;
    public int outerLocationLenght = 10;

    [Space(20)]
    public List<SpriteRenderer> paralaxFirstLayerBg;
    public List<SpriteRenderer> paralaxSecondLayerBg;
    public List<SpriteRenderer> paralaxUnderground;
    public List<RoadPart> paralaxGround;
    public List<SpriteRenderer> paralaxFront;

    [Space(20)]
    [Range(0, 1)]
    public float obstacleChance = 0.5f;
    public float obstacleHigh = -3.3f;

    [Space(20)]
    [Range(0, 1)]
    public float bonusChanse = 0.05f;
    [Space(20)]
    public int lootboxStartScore;
    public int lootBoxStawnChanse = 1;
    public int maxLootBoxByGame = 1;
    public LootBoxItemView lootBoxPrefab;

    [Space(20)]
    public int pepperSpawnChanse = 1;
    public BonusView pepperPrefab;
    [Space(20)]
    public int batSpawnChanse = 1;
    public BonusView batPrefab;

    [Space(20)]
    public ScriptableLocation guideLocation;
    public ScriptableLocation finishLocation;

    [Space(20)]
    public float stoppingTime = 1.2f;

    private ScriptableLocation currentLocation;
    private Queue<Sprite> roadQueue;
    private Queue<Sprite> frontQueue;
    private Queue<Sprite> undergroundQueue;

    private int currentBgOrdering;
    private int levelLength;

    private bool gameStarted;

    private float additionalBgSpeed;
    private float groundSpeed;
    private int guidObstacleNum = 0;

    private bool enableJumpObstacles = true;
    private bool enableSlipObstacles = true;
    private bool enableMixedObstacles = true;

    private bool levelSystem;

    private void Awake()
    {
        Instance = this;
        roadQueue = new Queue<Sprite>();
        frontQueue = new Queue<Sprite>();
        undergroundQueue = new Queue<Sprite>();
    }

    private void Start()
    {
        enableObstacleSpawn = true;
        DifficultyManger.OnDifficultyChange += OnDifficultyChange;

        currentBgOrdering = BgOrderingLayer1;
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

            ParalaxMove(paralaxFront, groundSpeed, frontQueue);
            ParalaxMove(paralaxUnderground, groundSpeed, undergroundQueue);
        }
    }

    public void StopMoving()
    {
        DifficultyManger.OnDifficultyChange -= OnDifficultyChange;
        StartCoroutine(StartStopping());
    }

    private IEnumerator StartStopping()
    {
        var currentStoppingTime = stoppingTime;
        var rate = 0.1f;

        while (currentStoppingTime > 0)
        {
            yield return new WaitForSeconds(rate);
            currentStoppingTime -= rate;
            var coof = currentStoppingTime / stoppingTime;
            groundSpeed = coof * paralaxGroundSpeed;
            additionalBgSpeed = coof * paralaxAdditionalBgSpeed;
        }
        groundSpeed = 0;
        additionalBgSpeed = 0;
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    public void ClearRoad()
    {
        foreach (var road in paralaxGround.Where(x => x.roadType != RoadType.LevelEnding))
        {
            road.Clear();
        }
    }

    public void ClearRoad(RoadPart except)
    {
        foreach (var road in paralaxGround.Where(x => x != except && x.roadType != RoadType.LevelEnding))
        {
            road.Clear();
        }
    }

    public void SetUpGenerator(GenerationCriteria criteria)
    {
        levelSystem = true;
        var startScreenCount = 3;
        speedDifferenceFactor = 1;

        batSpawnChanse = criteria.batSpawn ? batSpawnChanse : 0;
        pepperSpawnChanse = criteria.chillySpawn ? pepperSpawnChanse : 0;
        enableJumpObstacles = criteria.enableJumpObstacles;
        enableSlipObstacles = criteria.enableSlipObstacles;
        enableMixedObstacles = criteria.enableMixedObstacles;
        obstacleChance = criteria.obstacleChanse;

        levelLength = (int)(criteria.raceTime / (paralaxOffset / paralaxGroundSpeed)) + 1 - startScreenCount;

        locations.Clear();
        locations.AddRange(criteria.locations);
        if (!locations.Any())
        {
            Debug.LogError("Locations not found");
        }
        else if (locations.Count == 1)
        {
            innerLocationLenght = levelLength;
            outerLocationLenght = levelLength;
        }
        else
        {
            innerLocationLenght = criteria.innerLocationLength;
            outerLocationLenght = criteria.outerLocationLength;
        }
    }

    private void ParalaxMove(IEnumerable<SpriteRenderer> paralaxItems, float speed, Queue<Sprite> nextSpriteQueue = null)
    {
        var vectorSpeed = Vector3.left * speed * Time.deltaTime;
        var itemsCount = paralaxItems.Count();
        var itemMoveDistance = paralaxOffset * itemsCount / (itemsCount - 1);
        foreach (var item in paralaxItems)
        {
            if (item.transform.position.x < -itemMoveDistance)
            {
                item.transform.position += new Vector3(paralaxOffset * itemsCount, 0);
                if (nextSpriteQueue != null && nextSpriteQueue.Any())
                {
                    item.sprite = nextSpriteQueue.Dequeue();
                }
            }
            item.transform.position += vectorSpeed;
        }
    }

    private void GroundMove(IEnumerable<RoadPart> paralaxItems, float speed)
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
                    GenerateLocation();
                    if (levelSystem && levelLength == 0)
                    {
                        roadType = RoadType.LevelEnding;
                    }
                    else
                    {
                        roadType = RoadType.Start;

                    }
                }

                road.ChangeSprite(roadQueue.Dequeue());
                road.SetMaskOrder(currentBgOrdering);
                road.roadType = roadType;

                if (currentLocation.locationType == LocationType.Open)
                {
                    road.SetOrder(OpenRoadOrderingLayer);
                }
                else
                {
                    road.SetOrder(InnerRoadOrderingLayer);
                    road.EnableWalls();
                }

                if (GuideMapper.IsActive())
                {
                    GenerateGuideObstacle(road);
                }
                else if (enableObstacleSpawn && Random.value < obstacleChance)
                {
                    GenerateObstacle(road);
                }

                if (!GuideMapper.IsActive() && Random.value < bonusChanse)
                {
                    GenerateBonus(road);
                }
                else
                {
                    BeerManager.Instance.GenerateBeer(road);
                }
            }
            road.transform.position += vectorSpeed;
        }
    }

    private void GenerateLocation()
    {
        if (currentLocation == null)
        {
            if (GuideMapper.IsActive())
            {
                currentLocation = guideLocation;
            }
            else
            {
                currentLocation = locations.Where(x => x.locationType != LocationType.Inner).GetRandom();
            }
        }
        else
        {
            currentLocation = locations.Where(x => x.locationType != currentLocation.locationType).GetRandomOrDefault();
            currentLocation ??= locations.First();
        }

        if (currentBgOrdering == BgOrderingLayer1)
        {
            currentBgOrdering = BgOrderingLayer2;
            paralaxSecondLayerBg.ForEach(x => x.sprite = currentLocation.Background.GetRandom());
        }
        else
        {
            if (levelSystem && levelLength == 0)
            {
                currentLocation = finishLocation;
            }
            else
            {
                currentBgOrdering = BgOrderingLayer1;
                paralaxFirstLayerBg.ForEach(x => x.sprite = currentLocation.Background.GetRandom());
            }
        }


        roadQueue.Enqueue(currentLocation.startSprite);
        frontQueue.Enqueue(currentLocation.startFrontSprite);

        var locationLenght = currentLocation.locationType == LocationType.Inner ? innerLocationLenght : outerLocationLenght;
        locationLenght = Mathf.Clamp(locationLenght, 0, levelLength);
        levelLength -= locationLenght;

        for (var i = 0; i < locationLenght; i++)
        {
            undergroundQueue.Enqueue(currentLocation.Undergrounds.GetRandom());
            roadQueue.Enqueue(currentLocation.middleSprites.GetRandom());
            frontQueue.Enqueue(currentLocation.middleFrontSprites.GetRandomOrDefault());
        }
        roadQueue.Enqueue(currentLocation.finishSprite);
        frontQueue.Enqueue(currentLocation.finishFrontSprite);
    }

    private void GenerateBonus(RoadPart roadPart)
    {
        var currentLootBoxSpawnChanse = GameManager.score > lootboxStartScore && maxLootBoxByGame > 0 ? lootBoxStawnChanse : 0;
        var allBonusChanges = pepperSpawnChanse + currentLootBoxSpawnChanse + batSpawnChanse;

        var lootboxPersentChanse = (float)currentLootBoxSpawnChanse / allBonusChanges;
        var pepperPersentChanse = (float)pepperSpawnChanse / allBonusChanges;
        var batPersentChanse = (float)batSpawnChanse / allBonusChanges;

        GameObject spawnedBonus = null;
        var randomChanse = Random.value;
        if (randomChanse < lootboxPersentChanse)
        {
            maxLootBoxByGame--;
            spawnedBonus = Instantiate(lootBoxPrefab.gameObject, roadPart.transform);
        }
        else if (randomChanse < pepperPersentChanse + lootboxPersentChanse)
        {
            var spawnedPepper = Instantiate(pepperPrefab, roadPart.transform);
            spawnedPepper.OnBonusPickUp += () => ClearRoad(roadPart);
            spawnedBonus = spawnedPepper.gameObject;
        }
        else if (randomChanse < pepperPersentChanse + lootboxPersentChanse + batPersentChanse)
        {
            spawnedBonus = Instantiate(batPrefab.gameObject, roadPart.transform);
        }

        var bonusPosition = Vector2.zero;
        if (roadPart.obstacle != null)
        {
            var beerPatternType = roadPart.obstacle.beerPatterns.GetRandom();
            var beerPositions = beerPatternLibrary.beerPatterns.First(x => x.type == beerPatternType).beerPositions;
            bonusPosition = beerPositions[beerPositions.Count / 2];
        }
        else
        {
            bonusPosition = new Vector2(0, Random.Range(-3.3f, 3.3f));
        }

        bonusPosition = new Vector2(bonusPosition.x + roadPart.transform.position.x, bonusPosition.y);
        if (spawnedBonus != null)
        {
            spawnedBonus.transform.position = bonusPosition;
            roadPart.objectToRemove.Add(spawnedBonus.gameObject);
        }
    }

    private void GenerateObstacle(RoadPart roadPart)
    {
        var obstacle = currentLocation.obstacles
            .Where(obs => 
                (obs.obstacleType == ObstacleType.Jump && enableJumpObstacles)
                || (obs.obstacleType == ObstacleType.Slip && enableSlipObstacles)
                || (obs.obstacleType == ObstacleType.Mixed && enableMixedObstacles))
            .GetRandom();
        var spawnedObstacle = Instantiate(obstacle, roadPart.transform);
        spawnedObstacle.transform.position = new Vector3(roadPart.transform.position.x, obstacleHigh);

        roadPart.obstacle = spawnedObstacle;
        roadPart.objectToRemove.Add(spawnedObstacle.gameObject);
    }
    private void GenerateGuideObstacle(RoadPart roadPart)
    {
        if (guidObstacleNum < 3)
        {
            var obstacle = currentLocation.obstacles[guidObstacleNum];
            guidObstacleNum++;
            var spawnedObstacle = Instantiate(obstacle, roadPart.transform);
            spawnedObstacle.transform.position = new Vector3(roadPart.transform.position.x, obstacleHigh);

            roadPart.obstacle = spawnedObstacle;
            roadPart.objectToRemove.Add(spawnedObstacle.gameObject);
        }
    }

    private void OnDifficultyChange(float difficultyCoof)
    {
        groundSpeed = paralaxGroundSpeed * difficultyCoof * speedDifferenceFactor + paralaxGroundSpeed;
        additionalBgSpeed = paralaxAdditionalBgSpeed * difficultyCoof * speedDifferenceFactor + paralaxAdditionalBgSpeed;
    }
}
