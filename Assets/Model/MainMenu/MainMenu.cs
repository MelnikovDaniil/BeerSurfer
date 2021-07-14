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

    public HangerView hanger;
    public OutfitManager outfitManager;

    [NonSerialized]
    public bool shopIsOpen;


    private void Start()
    {
        shop.gameObject.SetActive(false);
        hanger.OnHangerClickEvent += OpenShop;
    }

    public void OpenGame()
    {
        shopIsOpen = false;
        shop.HideMenu();
        outfitManager.UpdateOutfits();
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

    private IEnumerator DelayShowGame()
    {
        yield return new WaitForSeconds(switchDelay);
        cameraManager.SetCameraBasePosition(switchMenuDelay);
    }

    private IEnumerator DelayShowLootBoxMenu()
    {
        shop.HideMenu();
        yield return new WaitForSeconds(switchDelay);
        cameraManager.SetTarget(lootboxMenuGameObject, 5, switchMenuDelay, Vector3.zero);
        cameraManager.OnReachesTargetEvnet += lootBoxManger.OpenMenu;
    }

    private IEnumerator DelayShowShopMenu()
    {
        lootBoxManger.HideMenu();
        yield return new WaitForSeconds(switchDelay);
        cameraManager.SetTarget(shopMenuGameObject, 5, switchMenuDelay, Vector3.zero);
        cameraManager.OnReachesTargetEvnet +=  shop.OpenMenu;
    }

    public void UpdateInfo()
    {
        discountText.transform.parent.gameObject.SetActive(false);
        scoreText.text = RecordMapper.Get().ToString();
        beerText.text = BeerMapper.Get().ToString();

        var discountCount = DobleBeerBonusMapper.Get();
        if (discountCount > 0)
        {
            discountText.transform.parent.gameObject.SetActive(true);
            discountText.text = discountCount.ToString();
        }
    }
}
