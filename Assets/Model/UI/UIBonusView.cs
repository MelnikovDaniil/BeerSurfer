using UnityEngine;
using UnityEngine.UI;

public class UIBonusView : MonoBehaviour
{
    public bool IsActive { get; private set; } = false;
    public Image frontIcon;
    public Image backIcon;
    public Animator animator;

    private TimedBuff _timedBuff;
    private float coof;

    public void FixedUpdate()
    {
        if (_timedBuff != null)
        {
            coof = _timedBuff.Duration / _timedBuff.Buff.duration;
            frontIcon.fillAmount = coof;
            if (_timedBuff.IsFinished)
            {
                Hide();
            }
        }
    }

    public void Show(TimedBuff timedBuff)
    {
        IsActive = true;
        gameObject.SetActive(true);
        animator.SetBool("active", true);
        _timedBuff = timedBuff;
        frontIcon.sprite = _timedBuff.Buff.sprite;
        backIcon.sprite = _timedBuff.Buff.sprite;
    }

    public void Hide()
    {
        IsActive = false;
        _timedBuff = null;
        animator.SetBool("active", false);
    }
}
