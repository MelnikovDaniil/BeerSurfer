using LionStudios.Suite.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelView : MonoBehaviour
{
    public Text beerCountText;
    public float deathAdTime = 3f;
    public Image adImage;
    public Button restartButton;
    public Button adButton;
    public Slider deathProgressBar;

    private float currentDeathAdTime;

    private void Update()
    {
        if (currentDeathAdTime > 0)
        {
            currentDeathAdTime -= Time.unscaledDeltaTime;
            adImage.fillAmount = currentDeathAdTime / deathAdTime;
            if (currentDeathAdTime <= 0)
            {
                DisableSeconLife();
            }
        }
    }

    public void ShowDeathPanel(int beerCount, int scoreCount)
    {
        var progress = -1f;
        if (GameManager.raceTime > 0)
        {
            progress = (Time.time - GameManager.startTime) / GameManager.raceTime;
            deathProgressBar.value = progress;
        }
        else
        {
            deathProgressBar.gameObject.SetActive(false);
        }
        var additionalData = new Dictionary<string, object>() { { "secondLife", GameManager.secondLifeActivated } };
        LionAnalytics.LevelFail(
            LevelManager.Instance.currentLevel,
            LevelMapper.GetAttempt(),
            (int)(progress * 100),
            additionalData: additionalData);
        currentDeathAdTime = deathAdTime;
        beerCountText.text = beerCount.ToString();
    }

    public void DisableSeconLife()
    {
        currentDeathAdTime = 0;
        restartButton.gameObject.SetActive(true);
        adButton.gameObject.SetActive(false);
    }
}
