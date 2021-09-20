using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Tooltip("Define loading level. -1 value setup level from save system")]
    public int currentLevel = -1;
    public List<LevelRangeConfiguration> levelRangeConfigurations;
    public TextMeshPro startText1;
    public TextMeshPro startText2;
    private GenerationCriteria lvlCriteria;

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

    public void StartLevel()
    {
        if (lvlCriteria != null)
        {
            StartCoroutine(FinishLevel(lvlCriteria.raceTime));
        }
    }

    private IEnumerator FinishLevel(float time)
    {
        yield return new WaitForSeconds(time);
        LevelMapper.Next();
        SceneManager.LoadScene("Game");
    }
}
