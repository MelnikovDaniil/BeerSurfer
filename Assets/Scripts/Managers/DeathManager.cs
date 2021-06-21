using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public DeathPanelView deathPanel;

    public void ShowDeathPanel()
    {
        gameObject.SetActive(true);
        var previousRecord = RecordMapper.Get();
        if (previousRecord < GameManager.score)
        {
            RecordMapper.Set(GameManager.score);
        }
        BeerMapper.Add(GameManager.beer);
        deathPanel.ShowDeathPanel(GameManager.beer, GameManager.score);
    }
}
