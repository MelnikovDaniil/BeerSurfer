using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("Define loading level. -1 value setup level from save system")]
    public int currentLevel = -1;
    public List<LevelRangeConfiguration> levelRangeConfigurations;

    public void Start()
    {
        if (currentLevel == -1)
        {
            
        }

        var lvlCriteria = levelRangeConfigurations
                .First(configuration =>
                    currentLevel >= configuration.levelRange.x
                    && currentLevel <= configuration.levelRange.y)
                .generationCriteria;
        LocationGenerator.Instance.SetUpGenerator(lvlCriteria);
    }
}
