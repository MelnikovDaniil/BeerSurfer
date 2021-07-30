using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIBonusView UIBonusPrefab;
    public int bonusCount;
    public Transform bonusPanel;

    private List<UIBonusView> bonuses;

    private void Awake()
    {
        Instance = this;
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
}
