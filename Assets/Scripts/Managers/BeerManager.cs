using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    public Color redColor;
    public Color redOutlineColor;
    public Color orangeColor;
    public Color orangeOutlineColor;

    [Space(20)]
    public BeerView beerPrefab;
    public float beerGroupLength = 10;
    public float maxBeerGroupLength = 20;
    public float beeerSpawnHeight = -3.3f;
    public MinMaxCurve minMaxBeerOnScreen = new MinMaxCurve(0, 4);
    public List<Sprite> beerSprites;


    [Space(20)]
    public Animator beerTextAnimator;

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

    [Space(20)]
    public float beerSoundResetTime = 0.5f;
    public float pitchInteration = 0.05f;

    private List<BeerView> pooledBeer;
    private float currentBeerGroupLenght;

    private float currentSoundPitch;
    private float currentSoundResetTime;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        DifficultyManger.OnDifficultyChange += OnDifficultyChange;
        character.OnBeerPickUpEvent += BeerPickUp;

        currentSoundResetTime = beerSoundResetTime;
        currentSoundPitch = 1;

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

    private void Update()
    {
        if (currentSoundResetTime > 0)
        {
            currentSoundResetTime -= Time.deltaTime;
            if (currentSoundResetTime <= 0)
            {
                currentSoundPitch = 1;
            }
        }
    }

    private void BeerPickUp(BeerView beer)
    {
        GameManager.beer++;
        SoundManager.PlaySound("beer").Source.pitch = currentSoundPitch;
        currentSoundResetTime = beerSoundResetTime;
        currentSoundPitch = Mathf.Clamp(currentSoundPitch + 0.1f, 1f, 2f);
        beer.Collect();
        var sprite = beer.GetSprite();
        uiBeerIcon.sprite = sprite;

        beerTextAnimator.Play("Pulse");

        if (sprite.name.Contains("Green"))
        {
            beerCounterText.color = greenColor;
            beerCounterOutline.effectColor = greenOutlineColor;
        }
        else if (sprite.name.Contains("Orange"))
        {
            beerCounterText.color = orangeColor;
            beerCounterOutline.effectColor = orangeOutlineColor;
        }
        else
        {
            beerCounterText.color = redColor;
            beerCounterOutline.effectColor = redOutlineColor;
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
        var beerGap = currentBeerGroupLenght / (beerCount + 1);

        for (var i = 0; i < beerCount; i++)
        {
            var beer = GetPooledBeer();
            beer.Enable();
            beer.transform.parent = roadPart.transform;
            beer.transform.position = new Vector2(x + roadPart.transform.position.x, beeerSpawnHeight);
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
                beer.transform.position = new Vector2(positions[i].x + roadPart.transform.position.x, positions[i].y);
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
                beer.transform.position = new Vector2(position.x + roadPart.transform.position.x, position.y);
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
