using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class LocationGenerator : MonoBehaviour
{
    public float paralaxOffset = 645;
    public float paralaxCloudsSpeed = 4;
    public float paralaxAdditionalBgSpeed = 4;
    public float paralaxGroundSpeed = 10;

    public List<Transform> paralaxAdditionalBg;
    public List<Transform> paralaxGround;
    public List<Transform> obstacles;
    public Transform obstacleContainer;

    public int obstaclesCount;
    public MinMaxCurve minMaxObstclesDistance = new MinMaxCurve(120, 250);
    public List<RectTransform> obstaclesPrefabs;
    public RectTransform finishPrefab;

    private const float StandartScreenCoefficient = 0.5625f;

    private bool gameStoped;

    private void Start()
    {
        var screenCoefficient = (float)Screen.height / Screen.width;
        paralaxOffset = paralaxOffset / StandartScreenCoefficient * screenCoefficient;
    }
    private void Update()
    {
        if (!gameStoped)
        {
            ParalaxMove(paralaxAdditionalBg, paralaxAdditionalBgSpeed);
            ParalaxMove(paralaxGround, paralaxGroundSpeed);
            obstacles.ForEach(x => x.position += Vector3.left * paralaxGroundSpeed * Time.deltaTime);
        }
    }

    public void ParalaxMove(IEnumerable<Transform> paralaxItems, float speed)
    {
        var vectorSpeed = Vector3.left * speed * Time.deltaTime;
        foreach (var item in paralaxItems)
        {
            if (item.position.x < -paralaxOffset)
            {
                item.position += new Vector3(paralaxOffset * 2, 0);
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
}
