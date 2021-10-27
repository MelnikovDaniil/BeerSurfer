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

    public Animator finishAnimator;
    public ParticleSystem finishParticles;
    public Text beerCountText;
    public float finishAdTime = 5f;
    public Image adImage;
    public Button finishContinueButton;
    public Button adButton;
    public int adReward = 150;


    public Text adRewardBonusText;
    public Text adRewardButtonText;
    private float currentFinishAdTime;


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

    private void Update()
    {
        if (currentFinishAdTime > 0)
        {
            currentFinishAdTime -= Time.unscaledDeltaTime;
            adImage.fillAmount = currentFinishAdTime / finishAdTime;
            if (currentFinishAdTime <= 0)
            {
                DisableAd();
            }
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
        Instance.finishAnimator.gameObject.SetActive(true);
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

    public static void FinishMenu()
    {
        Instance.ShowFinishMenu();
    }

    public void ShowFinishMenu()
    {
        currentFinishAdTime = finishAdTime;
        beerCountText.text = GameManager.beer.ToString();
        adRewardButtonText.text = "+" + adReward;
        adRewardBonusText.text = "+" + adReward;
        Instance.finishAnimator.SetTrigger("screenshot");
    }

    public void AcceptAd()
    {
        GameManager.beer += adReward;
        GameManager.adBonusBeer = adReward;
        adRewardBonusText.GetComponent<Animator>().SetTrigger("adReward");
        StartCoroutine(ChangeFinishBeerRoutine());

        DisableAd();
    }

    private IEnumerator ChangeFinishBeerRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        beerCountText.text = GameManager.beer.ToString();
    }

    public void DisableAd()
    {
        currentFinishAdTime = 0;
        finishContinueButton.gameObject.SetActive(true);
        adButton.gameObject.SetActive(false);
    }
}
