using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int score;
    public static int beer;
    public static bool isPaused;
    public Character character;

    public Text beerCounterText;
    public Text scoreCounterText;

    public Animator pausePanelAnimtor;
    public float continueDelay = 3f;
    public LocationGenerator locationGenerator;
    public MainMenu mainMenu;
    public DeathManager deathPanel;
    public Slider volumeSlider;
    public CanvasGroup uiCanvasGroup;
    public HangerView hangerView;

    [Space(20)]
    public float secondLifeDelay = 2;

    [Space(20)]
    public List<AudioClip> music;

    private bool gameStarted;
    private bool gameEnded;
    private float timeScale;

    private Coroutine continueCoroutine;

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
        uiCanvasGroup.blocksRaycasts = false;
        SceneManager.sceneUnloaded += SceenLoaded;
    }

    private void SceenLoaded(Scene arg0)
    {
        var previousRecord = RecordMapper.Get();
        if (previousRecord < score)
        {
            RecordMapper.Set(score);
        }
        var finalBeer = BeerManager.Instance.doubleBeerBonus ? beer * 2 : beer;
        BeerMapper.Add(finalBeer);
        Time.timeScale = 1;
        isPaused = false;
        beer = 0;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("ShopAvailable", 0) != 0 || LootBoxMapper.Get() > 0 || BeerMapper.Get() >= 300)
        {
            hangerView.Enable();
            PlayerPrefs.SetInt("ShopAvailable", 1);
        }
        else
        {
            hangerView.Disable();
        }
    }

    private void StartGame()
    {
        if (!mainMenu.shopIsOpen)
        {
            gameStarted = true;
            uiCanvasGroup.interactable = true;
            uiCanvasGroup.blocksRaycasts = true;
            character.OnJumpEvent -= StartGame;
            character.isStartedRun = true;
            locationGenerator.StartGame();
            SoundManager.PlayMusic(music.GetRandom().name);

            if (DobleBeerBonusMapper.Get() > 0)
            {
                character.EnableDobleBeerBonus();
                DobleBeerBonusMapper.RemoveOne();
                BeerManager.Instance.doubleBeerBonus = true;
            }
        }
    }

    public void FixedUpdate()
    {
        if (gameStarted && !gameEnded)
        {
            score += 1;
            scoreCounterText.text = score.ToString();
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
            if ((Time.timeScale - Time.deltaTime) > 0)
            {
                Time.timeScale -= Time.deltaTime;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
    }

    public void SecondLife()
    {
        Time.timeScale = 0;
        character.SecondLife(secondLifeDelay);
        LocationGenerator.Instance.ClearRoad();
        UIManager.Blind();
        StartCoroutine(SecondLifeEnd());
    }

    public IEnumerator SecondLifeEnd()
    {
        yield return new WaitForSecondsRealtime(secondLifeDelay);
        gameEnded = false;
        Time.timeScale = 1;
    }

    private void StopPoits()
    {
        gameEnded = true;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }



    public void PauseOrResume()
    {
        if (!gameEnded)
        {
            volumeSlider.value = GetVolume();
            var paused = !isPaused;
            if (paused)
            {
                if (continueCoroutine != null)
                {
                    StopCoroutine(continueCoroutine);
                }

                pausePanelAnimtor.gameObject.SetActive(true);
                if (Time.timeScale != 0)
                {
                    timeScale = Time.timeScale;
                }
                Time.timeScale = 0f;
                isPaused = paused;
            }
            else
            {
                continueCoroutine = StartCoroutine(GameContinue());
            }
            pausePanelAnimtor.SetBool("pause", paused);
        }
    }

    public IEnumerator GameContinue()
    {
        yield return new WaitForSecondsRealtime(continueDelay);
        pausePanelAnimtor.gameObject.SetActive(false);
        isPaused = false;
        Time.timeScale = timeScale;
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
