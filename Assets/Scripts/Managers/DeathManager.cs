using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public DeathPanelView deathPanel;

    public void ShowDeathPanel()
    {
        gameObject.SetActive(true);
        var beer = BeerManager.Instance.doubleBeerBonus ? GameManager.beer * 2 : GameManager.beer;
        deathPanel.ShowDeathPanel(beer, GameManager.score);
    }
}
