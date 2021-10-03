using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelView : MonoBehaviour
{
    public Text beerCountText;
    public Text scoreCountText;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ShowDeathPanel(int beerCount, int scoreCount)
    {
        beerCountText.text = beerCount.ToString();
        scoreCountText.text = "score: " + scoreCount.ToString();
    }
}
