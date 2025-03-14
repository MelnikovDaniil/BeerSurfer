using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Space(20)]
    public CameraManager cameraManager;
    public float switchDelay = 1f;

    public Shop shop;
    public LootBoxManger lootBoxManger;
    public GameObject shopMenuGameObject;
    public GameObject lootboxMenuGameObject;
    public float switchMenuDelay = 0.3f;

    public Text beerText;
    public Text scoreText;
    public Text discountText;
    public Text batText;

    public HangerView hanger;
    public OutfitManager outfitManager;

    public Animator shopCharacterAnimator;

    [NonSerialized]
    public bool shopIsOpen;


    private void Start()
    {
        shopCharacterAnimator.SetTrigger("shop");
        shop.gameObject.SetActive(false);
        hanger.OnHangerClickEvent += OpenShop;
        outfitManager.ResetOutfits();
        SoundManager.PlayMusic("MenuMusic");
    }

    public void OpenGame()
    {
        SoundManager.PlayMusic("MenuMusic");
        shopIsOpen = false;
        shop.HideMenu();
        outfitManager.ResetOutfits();
        StartCoroutine(DelayShowGame());
        //cameraManager.OnReachesTargetEvnet += () => shop.gameObject.SetActive(false);
    }

    private void OpenShop()
    {
        shopIsOpen = true;
        cameraManager.SetTarget(shopMenuGameObject, cameraManager.camera.orthographicSize, switchMenuDelay, Vector3.zero);
        cameraManager.OnReachesTargetEvnet += () =>
        {
            shop.gameObject.SetActive(true);
            shop.OpenMenu();
            UpdateInfo();
            SoundManager.PlayMusic("lift");
        };
    }

    public void ShowLootBoxMenu()
    {
        StartCoroutine(DelayShowLootBoxMenu());
        UpdateInfo();
    }

    public void ShowShopMenu()
    {
        StartCoroutine(DelayShowShopMenu());
        UpdateInfo();
    }

    public void Close()
    {
        shopMenuGameObject.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private IEnumerator DelayShowGame()
    {
        yield return new WaitForSeconds(switchDelay);
        cameraManager.SetCameraBasePosition(switchMenuDelay);
    }

    private IEnumerator DelayShowLootBoxMenu()
    {
        shop.HideMenu();
        yield return new WaitForSeconds(switchDelay);
        cameraManager.SetTarget(lootboxMenuGameObject, switchMenuDelay, Vector3.zero);
        cameraManager.OnReachesTargetEvnet += lootBoxManger.OpenMenu;
    }

    private IEnumerator DelayShowShopMenu()
    {
        lootBoxManger.HideMenu();
        yield return new WaitForSeconds(switchDelay);
        cameraManager.SetTarget(shopMenuGameObject, switchMenuDelay, Vector3.zero);
        cameraManager.OnReachesTargetEvnet +=  shop.OpenMenu;
    }

    public void UpdateInfo()
    {
        discountText.transform.parent.gameObject.SetActive(false);
        batText.transform.parent.gameObject.SetActive(false);
        scoreText.text = RecordMapper.Get().ToString();
        beerText.text = BeerMapper.Get().ToString();
        
        var discountCount = DobleBeerBonusMapper.Get();
        if (discountCount > 0)
        {
            discountText.transform.parent.gameObject.SetActive(true);
            discountText.text = discountCount.ToString();
        }

        var batCount = BatBonusMapper.Get();
        if (batCount > 0)
        {
            batText.transform.parent.gameObject.SetActive(true);
            batText.text = batCount.ToString();
        }
    }
}
