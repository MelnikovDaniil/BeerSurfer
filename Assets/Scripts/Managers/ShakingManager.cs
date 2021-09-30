using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShakingManager : MonoBehaviour
{
    public static ShakingManager Instance;
    public float maxShakingTime = 5;
    public int maxTabs = 30;
    public float maxSplashForce;
    public float maxAnimationSpeed = 2;
    public Transform handTransform;
    public ParticleSystem splashParticles;
    public Vector3 splashCameraShift;

    [Space(20)]
    public float bonusStartPosition = 7.5f;
    public float bonusGap = 11.5f;
    public TextMeshPro bonusText;
    public List<int> rewardList;

    private Animator _characterAnimator;
    private bool shaking;
    private float boostStep;
    private int tabs;
    private ParticleSystem particle;
    private Rigidbody2D particleRigidbody;
    private float nextBonusPosition;
    private Queue<int> rewardQueue;

    private void Awake()
    {
        Instance = this;
        _characterAnimator = GetComponent<Animator>();
        boostStep = maxAnimationSpeed / maxTabs;
        rewardQueue = new Queue<int>(rewardList);
    }

    private void Update()
    {
        if (shaking)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var speed = _characterAnimator.GetFloat("shakingSpeed") + boostStep;
                _characterAnimator.SetFloat("shakingSpeed", speed);
                tabs += 1;
                if (tabs >= maxTabs)
                {
                    Splash();
                }
            }
        }

        if (particle != null)
        {
            if (particle.transform.position.y > nextBonusPosition)
            {
                nextBonusPosition += bonusGap;
                var text = Instantiate(bonusText, particle.transform.position + new Vector3(2, 0), Quaternion.identity);
                var beer = rewardQueue.Dequeue();
                GameManager.beer += beer;
                text.text = $"+{beer}";
            }

            var main = particle.main;
            main.startLifetimeMultiplier = Mathf.Clamp((particle.transform.position.y - handTransform.position.y) / 5f, 0, 4f);

            if (particleRigidbody.velocity.y < 0.1f)
            {
                particle = null;
                UIManager.Sceenshot();
            }

        }
    }

    public void StartShaking()
    {
        shaking = true;
        tabs = 0;
        StartCoroutine(FinishShaking());
    }
    private IEnumerator FinishShaking()
    {
        yield return new WaitForSeconds(maxShakingTime);
        Splash();
    }

    private void Splash()
    {
        if (shaking)
        {
            nextBonusPosition = bonusStartPosition;
            _characterAnimator.Play("SodaSplash");
            shaking = false;
            StartCoroutine(ThrowParticles());
        }
    }

    private IEnumerator ThrowParticles()
    {
        yield return new WaitForSeconds(0.25f);
        particle = Instantiate(splashParticles, handTransform.transform.position, Quaternion.identity);
        particleRigidbody = particle.GetComponent<Rigidbody2D>();
        var splashVector = Vector2.up * tabs / maxTabs * maxSplashForce;
        particleRigidbody.AddForce(splashVector, ForceMode2D.Impulse);
        CameraManager.Instance.SetTarget(particle.gameObject, -1, splashCameraShift);
    }
}
