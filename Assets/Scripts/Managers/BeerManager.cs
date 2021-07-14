using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Extension;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;

public class BeerManager : MonoBehaviour
{
    public static BeerManager Instance;

    public Character character;
    public BeerPatternLibrary beerPatternLibrary;

    [Space(20)]
    public Color greenColor;
    public Color greenOutlineColor;
    public Color brownColor;
    public Color brownOutlineColor;

    [Space(20)]
    public BeerView beerPrefab;
    public float beerGroupLength = 10;
    public float maxBeerGroupLength = 20;
    public float beeerSpawnHeight = -3.3f;
    public MinMaxCurve minMaxBeerOnScreen = new MinMaxCurve(0, 4);
    public List<Sprite> beerSprites;

    [Space(20)]
    [SerializeField]
    [Range(0, 1)]
    private float everySecondBeerChance = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    private float allBeerChance = 0.15f;

    [Space(20)]
    public int amountOfPool = 10;

    [Space(20)]
    public Image uiBeerIcon;
    public Text beerCounterText;
    public Outline beerCounterOutline;

    [Space(20)]
    public Sprite doubleBeerSprite;
    public bool doubleBeerBonus;

    private List<BeerView> pooledBeer;
    private float currentBeerGroupLenght;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        DifficultyManger.OnDifficultyChange += OnDifficultyChange;
        character.OnBeerPickUpEvent += BeerPickUp;


        currentBeerGroupLenght = beerGroupLength;
        var pooledBeerObject = new GameObject("pooledBeer")
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        pooledBeer = new List<BeerView>();
        BeerView beer;
        for (var i = 0; i < amountOfPool; i++)
        {
            beer = Instantiate(beerPrefab, pooledBeerObject.transform);
            pooledBeer.Add(beer);
            beer.gameObject.SetActive(false);
        }
    }

    private void BeerPickUp(BeerView beer)
    {
        GameManager.beer++;
        SoundManager.PlaySound("beer");
        beer.Collect();
        var sprite = beer.GetSprite();
        uiBeerIcon.sprite = sprite;

        if (sprite.name.Contains("Green"))
        {
            beerCounterText.color = greenColor;
            beerCounterOutline.effectColor = greenOutlineColor;
        }
        else
        {
            beerCounterText.color = brownColor;
            beerCounterOutline.effectColor = brownOutlineColor;
        }
    }


    public void GenerateBeer(RoadPart roadPart)
    {
        if (roadPart.obstacle == null)
        {
            GenerateWithoutObstacles(roadPart);
        }
        else
        {
            GenerateWithObstacle(roadPart);
        }
    }

    private void GenerateWithoutObstacles(RoadPart roadPart)
    {
        var beerSprite = GetBeerSprite();
        var beerCount = Random.Range(minMaxBeerOnScreen.constantMin, minMaxBeerOnScreen.constantMax);

        var x = -currentBeerGroupLenght / 2;
        var beerGap = currentBeerGroupLenght / (beerCount - 1);

        for (var i = 0; i < beerCount; i++)
        {
            var beer = GetPooledBeer();
            beer.Enable();
            beer.transform.parent = roadPart.transform;
            beer.transform.localPosition = new Vector2(x, beeerSpawnHeight);
            beer.ChangeSprite(beerSprite);
            roadPart.objectToRemove.Add(beer.gameObject);
            x += beerGap;
        }
    }

    private void GenerateWithObstacle(RoadPart roadPart)
    {
        var beerSprite = GetBeerSprite();
        var generationPattern = roadPart.obstacle.beerPatterns.GetRandom();
        var positions = GetBeerPositioForObstacle(generationPattern).ToList();

        var chanse = Random.value;
        if (chanse < everySecondBeerChance)
        {
            for (var i = 0; i < positions.Count; i += 2)
            {
                var beer = GetPooledBeer();
                beer.Enable();
                beer.transform.parent = roadPart.transform;
                beer.transform.localPosition = positions[i];
                roadPart.objectToRemove.Add(beer.gameObject);
                beer.ChangeSprite(beerSprite);
            }
        }
        else if (chanse < allBeerChance)
        {
            foreach (var position in positions)
            {
                var beer = GetPooledBeer();
                beer.Enable();
                beer.transform.parent = roadPart.transform;
                beer.transform.localPosition = position;
                roadPart.objectToRemove.Add(beer.gameObject);
                beer.ChangeSprite(beerSprite);
            }
        }
    }

    private IEnumerable<Vector2> GetBeerPositioForObstacle(BeerPatternType patternType)
    {
        var positions = beerPatternLibrary.beerPatterns.First(x => x.type == patternType).beerPositions;
        var x = -currentBeerGroupLenght / 2f;
        var beerGap = currentBeerGroupLenght / (positions.Count - 1);
        for (var i = 0; i < positions.Count; i++)
        {
            positions[i] = new Vector2(x, positions[i].y);
            x += beerGap;
        }

        return positions;
    }

    private BeerView GetPooledBeer()
    {
        return pooledBeer.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
    }

    private void OnDifficultyChange(float difficultyCoof) 
    {
        currentBeerGroupLenght = (maxBeerGroupLength - beerGroupLength) * difficultyCoof + beerGroupLength;
    }

    private Sprite GetBeerSprite()
    {
        if (doubleBeerBonus)
        {
            return doubleBeerSprite;
        }

        return beerSprites.GetRandom();
    }
}
