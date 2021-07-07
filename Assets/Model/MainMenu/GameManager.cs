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

    public GameObject pausePanel;
    public LocationGenerator locationGenerator;
    public MainMenu mainMenu;
    public DeathManager deathPanel;
    public Slider volumeSlider;
    public CanvasGroup uiCanvasGroup;

    private bool gameStarted;
    private bool gameEnded;
    private bool isPaused;
    private float timeScale;

    private void Awake()
    {
        score = 0;
        beer = 0;
        gameEnded = false;
        character.OnDeathEvent += StopPoits;
        character.OnDeathEvent += deathPanel.ShowDeathPanel;
        character.OnJumpEvent += StartGame;
        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = GetVolume();
        uiCanvasGroup.alpha = 0;
        uiCanvasGroup.interactable = false;
    }

    private void StartGame()
    {
        if (!mainMenu.shopIsOpen)
        {
            gameStarted = true;
            uiCanvasGroup.interactable = true;
            character.OnJumpEvent -= StartGame;
            character.isStartedRun = true;
            locationGenerator.StartGame();
            if (Random.value <= 0.10f)
            {
                SoundManager.PlayMusic("dejavu");
            }
            else
            {
                SoundManager.PlayMusic("naruto");
            }
        }
    }

    public void FixedUpdate()
    {
        if (gameStarted)
        {
            score += 1;
            scoreCounterText.text = $"score: {score}";
            beerCounterText.text = beer.ToString();
        }
    }
    public void Update()
    {
        if (gameStarted && uiCanvasGroup.alpha < 1)
        {
            uiCanvasGroup.alpha += Time.deltaTime * 2;
        }

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



    public void PauseOrResume()
    {
        volumeSlider.value = GetVolume();
        isPaused = !isPaused;
        if (isPaused)
        {
            pausePanel.SetActive(true);
            if (Time.timeScale != 0)
            {
                timeScale = Time.timeScale;
            }
            Time.timeScale = 0f;

        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = timeScale;
        }
    }

    private void OnApplicationPause(bool pause)
    {

        if (gameStarted && pause)
        {
            isPaused = false;
            PauseOrResume();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (gameStarted && !focus)
        {
            isPaused = false;
            PauseOrResume();
        }
    }

    private void SetVolume(float volume)
    {
        SoundManager.SetMusicVolume(volume);
        SoundManager.SetSoundVolume(volume);
    }

    private float GetVolume()
    {
        return SoundManager.GetSoundVolume();
    }

}
