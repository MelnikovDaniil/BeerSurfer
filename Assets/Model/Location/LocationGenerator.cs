using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LocationGenerator : MonoBehaviour
{
    public float paralaxOffset = 645;
    public float paralaxCloudsSpeed = 4;
    public float paralaxAdditionalBgSpeed = 4;
    public float paralaxGroundSpeed = 10;

    [Space(20)]
    public List<ScriptableLocation> locations;
    public int locationLenght = 10;

    [Space(20)]
    public List<Transform> paralaxAdditionalBg;
    public List<Transform> paralaxGround;
    public List<Transform> obstacles;
    public Transform obstacleContainer;

    [Space(20)]
    public int obstaclesCount;
    public MinMaxCurve minMaxObstclesDistance = new MinMaxCurve(120, 250);
    public List<RectTransform> obstaclesPrefabs;
    public RectTransform finishPrefab;

    private Queue<Sprite> roadQueue;
    private Queue<Sprite> backgroundQueue;

    private const float StandartScreenCoefficient = 0.5625f;

    private bool gameStoped;

    private void Start()
    {
        roadQueue = new Queue<Sprite>();
        backgroundQueue = new Queue<Sprite>();
    }

    private void Update()
    {
        if (!gameStoped)
        {
            ParalaxMove(paralaxAdditionalBg, backgroundQueue, paralaxAdditionalBgSpeed);
            ParalaxMove(paralaxGround, roadQueue, paralaxGroundSpeed);
            obstacles.ForEach(x => x.position += Vector3.left * paralaxGroundSpeed * Time.deltaTime);
        }
    }

    public void ParalaxMove(IEnumerable<Transform> paralaxItems, Queue<Sprite> queue, float speed)
    {
        var vectorSpeed = Vector3.left * speed * Time.deltaTime;
        var itemsCount = paralaxItems.Count();
        var itemMoveDistance = paralaxOffset * itemsCount / (itemsCount - 1);
        foreach (var item in paralaxItems)
        {
            if (item.position.x < -itemMoveDistance)
            {
                item.position += new Vector3(paralaxOffset * itemsCount, 0);

                if (!queue.Any())
                {
                    GenerateLocation();
                }

                item.GetComponent<SpriteRenderer>().sprite = queue.Dequeue();
            }
            item.position += vectorSpeed;
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
        for (var i = 0; i < (locationLenght + 2) / 2; i++)
        {
            backgroundQueue.Enqueue(location.Background);
        }

        roadQueue.Enqueue(location.startSprite);
        
        for (var i = 0; i < locationLenght; i++)
        {
            roadQueue.Enqueue(location.middleSprites.GetRandom());
        }
        roadQueue.Enqueue(location.finishSprite);
    }
}
