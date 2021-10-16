using LionStudios.Suite.Analytics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Tooltip("Define loading level. -1 value setup level from save system")]
    public int currentLevel = -1;
    public List<LevelRangeConfiguration> levelRangeConfigurations;
    public TextMeshPro startText1;
    public TextMeshPro startText2;
    public GenerationCriteria lvlCriteria;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        if (currentLevel == -1)
        {
            currentLevel = LevelMapper.Get();
        }

        lvlCriteria = levelRangeConfigurations
                .FirstOrDefault(configuration =>
                    currentLevel >= configuration.levelRange.x
                    && currentLevel <= configuration.levelRange.y)
                ?.generationCriteria;

        string text;
        if (lvlCriteria != null)
        {
            text = "LVL " + currentLevel;

            Random.InitState(512512521 + currentLevel);
            LocationGenerator.Instance.SetUpGenerator(lvlCriteria);
        }
        else
        {
            text = "Infinity";
        }
        LevelMapper.AddAttempt();
        LionAnalytics.LevelStart(currentLevel, LevelMapper.GetAttempt(), 0);
        startText1.text = text;
        startText2.text = text;
    }

    public static void RestartLevel()
    {
        var additionalData = new Dictionary<string, object>() { { "secondLife", GameManager.secondLifeActivated } };
        LionAnalytics.LevelRestart(
            Instance.currentLevel,
            LevelMapper.GetAttempt(),
            (int)((Time.time - GameManager.startTime) / GameManager.raceTime * 100),
            additionalData: additionalData);
    }

    public static void FinishLevel()
    {
        var additionalData = new Dictionary<string, object>() { { "secondLife", GameManager.secondLifeActivated } };
        var collectedBeer = new VirtualCurrency("Collected Soda", "Basic", GameManager.colectedBeer);
        var minigameBeer = new VirtualCurrency("Minigame Soda", "Basic", GameManager.minigameBeer);
        var adBeer = new VirtualCurrency("Ad Soda", "Basic", GameManager.adBonusBeer);
        var product = new Product();
        product.AddVirtualCurrency(collectedBeer);
        product.AddVirtualCurrency(minigameBeer);
        product.AddVirtualCurrency(adBeer);

        var reward = new Reward(product)
        {
            rewardName = "Finish Reward"
        };
        LionAnalytics.LevelComplete(
            Instance.currentLevel,
            LevelMapper.GetAttempt(),
            100,
            reward,
            additionalData: additionalData);
        LevelMapper.ResetAttempts();
        LevelMapper.Next();
        LionAnalytics.SetPlayerLevel(LevelMapper.Get());
    }
}
