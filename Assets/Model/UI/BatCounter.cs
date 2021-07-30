using UnityEngine;
using UnityEngine.UI;

public class BatCounter : MonoBehaviour
{
    public Image frontIcon;
    public Text batCount;
    public Animator animator;

    private float currentTime;
    private float duration;
    private float coof;

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            coof = currentTime / duration;
            frontIcon.fillAmount = coof;
            if (currentTime < 0)
            {
                Hide();
            }
        }
    }


    public void SetCooldown(float time)
    {
        gameObject.SetActive(true);
        animator.SetBool("active", true);
        duration = time;
        currentTime = duration;
        batCount.text = BatBonusMapper.Get().ToString();
    }

    public void Hide()
    {
        animator.SetBool("active", false);
    }
}
