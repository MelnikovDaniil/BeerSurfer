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

        startText1.text = text;
        startText2.text = text;
    }

    public static void FinishLevel()
    {
        LevelMapper.Next();
    }
}
