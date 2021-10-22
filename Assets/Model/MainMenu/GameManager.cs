using com.adjust.sdk;
using GameAnalyticsSDK;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Debugging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int score;
    public static int beer;

    public static int colectedBeer;
    public static int minigameBeer;
    public static int adBonusBeer;
    public static bool isPaused;
    public static float startTime;
    public static float raceTime;
    public static bool secondLifeActivated;
    public Character character;

    public Text beerCounterText;
    public Text scoreCounterText;

    public Animator pausePanelAnimtor;
    public float continueDelay = 3f;
    public LocationGenerator locationGenerator;
    public MainMenu mainMenu;
    public DeathManager deathPanel;
    public CanvasGroup uiCanvasGroup;
    public HangerView hangerView;

    [Space(20)]
    public Button pauseMuteButton;
    public Sprite MuteOn;
    public Sprite MuteOff;

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
#if UNITY_IOS
        /* Mandatory - set your iOS app token here */
        InitAdjust("y5vpikc3vbb4");
#elif UNITY_ANDROID
        /* Mandatory - set your Android app token here */
        InitAdjust("y5vpikc3vbb4");
#endif
        GameAnalytics.Initialize();
        LionAnalytics.GameStart();
        Instance = this;
        score = 0;
        beer = 0;
        raceTime = -1;
        gameEnded = false;
        character.OnDeathEvent += StopPoits;
        character.OnDeathEvent += deathPanel.ShowDeathPanel;
        character.OnJumpEvent += StartGame;
        pauseMuteButton.onClick.AddListener(Mute);
        pauseMuteButton.onClick.AddListener(SetMuteIcon);
        uiCanvasGroup.alpha = 0;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;
        SceneManager.sceneUnloaded += SceenLoaded;
    }
    private void Start()
    {
        LionDebugger.Hide();
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


    private void StartGame()
    {
        if (!mainMenu.shopIsOpen)
        {
            mainMenu.Close();
            startTime = Time.time;
            gameStarted = true;
            uiCanvasGroup.interactable = true;
            uiCanvasGroup.blocksRaycasts = true;
            character.OnJumpEvent -= StartGame;
            character.isStartedRun = true;
            locationGenerator.StartGame();
            SoundManager.PlayMusic(music.GetRandom().name);
            GuideManager.Instance.StartManager();
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
        secondLifeActivated = true;
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

    public void StopPoits()
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
            SetMuteIcon();
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

        if (gameStarted && !gameEnded && pause)
        {
            isPaused = false;
            PauseOrResume();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (gameStarted && !gameEnded && !focus)
        {
            isPaused = false;
            PauseOrResume();
        }
    }

    public void Mute()
    {
        if (SoundManager.GetMusicMuted())
        {
            SoundManager.SetMusicMuted(false);
            SoundManager.SetSoundMuted(false);
        }
        else
        {
            SoundManager.SetMusicMuted(true);
            SoundManager.SetSoundMuted(true);
        }
    }

    public void SetMuteIcon()
    {
        pauseMuteButton.image.sprite = SoundManager.GetMusicMuted() ? MuteOn : MuteOff;
    }

    private void InitAdjust(string adjustAppToken)
    {
        var adjustConfig = new AdjustConfig(
            adjustAppToken,
            AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
            true
        );
        adjustConfig.setLogLevel(AdjustLogLevel.Verbose); // AdjustLogLevel.Suppress to disable logs
        adjustConfig.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename

        // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters

        //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
        //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
        //});

        Adjust.start(adjustConfig);

    }
}
