using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIBonusView UIBonusPrefab;
    public int bonusCount;
    public Transform bonusPanel;
    public Image screenshotPlace;

    public Animator finishAnimator;
    public ParticleSystem finishParticles;
    public Toggle likesToggle;
    public Text finishLikeText;
    public Text finishCommetsText;
    public Text finishTagsText;
    public Button finishContinueButton;
    public float continueDelayTime = 3f;

    private Animator _animator;
    private List<UIBonusView> bonuses;

    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        bonuses = new List<UIBonusView>();
        for (var i = 0; i < bonusCount; i++)
        {
            var bonus = Instantiate(UIBonusPrefab, bonusPanel);
            bonus.gameObject.SetActive(false);
            bonuses.Add(bonus);
        }
    }

    public static void AddBuff(TimedBuff timedBuff)
    {
        var view = Instance.bonuses.First(x => !x.IsActive);
        view.Show(timedBuff);
    }

    public static void Blind()
    {
        Instance._animator.SetTrigger("blind");
    }

    public static void Finish()
    {
        Instance.finishAnimator.SetTrigger("finish");
        Instance.finishParticles.Play();
    }

    public static void StartShaking()
    {
        Instance.finishAnimator.SetTrigger("shaking");
    }

    public static void FinishShaking()
    {
        Instance.finishAnimator.SetTrigger("shaking");
    }

    public static void Sceenshot()
    {
        Instance.screenshotPlace.sprite = ScreenshotManger.TakeScreenshot(Camera.main.pixelWidth, Camera.main.pixelWidth * 2 / 3);
        Instance.StartCoroutine(Instance.ContinueWithDelayRoutine());
        Instance.finishTagsText.text = $"#lvl{LevelManager.Instance.currentLevel} {Instance.finishTagsText.text}";
        Instance.finishAnimator.SetTrigger("screenshot");
        Instance.StartCoroutine(Instance.LikesAndCommentRouting());
    }

    public void Like()
    {
        var likes = int.Parse(finishLikeText.text);
        if (likesToggle.isOn)
        {
            likes++;
        }
        else
        {
            likes--;
        }
        finishLikeText.text = likes.ToString();

    }

    private IEnumerator ContinueWithDelayRoutine()
    {
        finishContinueButton.interactable = false;
        yield return new WaitForSecondsRealtime(continueDelayTime);
        finishContinueButton.interactable = true;
    }

    private IEnumerator LikesAndCommentRouting()
    {
        var comments = 0;
        var likes = 0;
        for (var i = 0; i < 10; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            comments += Random.Range(0, 100);
            likes += Random.Range(0, 100);
            finishCommetsText.text = comments.ToString();
            finishLikeText.text = likes.ToString();
        }
    }
}
