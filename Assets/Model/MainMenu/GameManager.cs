using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int score;
    public static int beer;
    public Character character;

    public Text beerCounterText;
    public Text scoreCounterText;

    private bool gameEnded;

    private void Awake()
    {
        score = 0;
        beer = 0;
        gameEnded = false;
        character.OnDeathEvent += StopPoits;
    }

    public void FixedUpdate()
    {
        score += 1;
        scoreCounterText.text = $"score: {score}";
        beerCounterText.text = beer.ToString();
    }
    public void Update()
    {
        if (gameEnded)
        {
            if ((Time.timeScale - Time.fixedDeltaTime) > 0)
            {
                Time.timeScale -= Time.fixedDeltaTime;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
    }


    private void StopPoits()
    {
        gameEnded = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }
}
