using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public DeathPanelView deathPanel;

    public void ShowDeathPanel()
    {
        gameObject.SetActive(true);
        deathPanel.ShowDeathPanel(GameManager.beer, GameManager.score);
    }
}
