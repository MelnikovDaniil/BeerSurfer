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

    public static void Sceenshot()
    {
        Instance.screenshotPlace.sprite = ScreenshotManger.TakeScreenshot(Camera.main.pixelWidth, Camera.main.pixelWidth * 2 / 3);
        Instance.finishAnimator.SetTrigger("screenshot");
    }
}
