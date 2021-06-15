using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public DeathPanelView deathPanel;

    public Character character;

    private void Awake()
    {
        gameObject.SetActive(false);
        character.OnDeathEvent += ShowDeathPanel;
    }

    public void ShowDeathPanel()
    {
        gameObject.SetActive(true);
        deathPanel.ShowDeathPanel(GameManager.beer, GameManager.score);
    }
}
