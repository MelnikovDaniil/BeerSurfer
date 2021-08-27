using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance;

    private Animator _animator;
    private bool isWaitingForResponce;
    private GuideSteps waitingStep;
    private bool isActive;

    private void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void StartManager()
    {
        isActive = GuideMapper.IsActive();
        if (isActive)
        {
            GuideMapper.SetStep(0);
        }
    }

    public bool WhaitingForOrNotGuide(GuideSteps steps)
    {
        if (!isActive)
        {
            return true;
        }

        return isWaitingForResponce && waitingStep == steps;
    }

    public void ActivateStep()
    {
        var step = GuideMapper.GetStep();

        Time.timeScale = 0;
        gameObject.SetActive(true);
        switch (step)
        {
            case 0:
                waitingStep = GuideSteps.Jump;
                _animator.SetTrigger("jump");
                break;
            case 1:
                waitingStep = GuideSteps.Slip;
                _animator.SetTrigger("slip");
                break;
            case 2:
                waitingStep = GuideSteps.Bat;
                _animator.SetTrigger("bat");
                break;
        }
        isWaitingForResponce = true;
    }


    public void FinishStep()
    {
        if (isWaitingForResponce)
        {
            Time.timeScale = 1;
            var step = GuideMapper.GetStep();
            if (step == 2)
            {
                GuideMapper.SetActive(false);
                isActive = false;
            }
            else
            {
                GuideMapper.SetStep(step + 1);
            }

            gameObject.SetActive(false);
            isWaitingForResponce = false;
        }
    }
}


