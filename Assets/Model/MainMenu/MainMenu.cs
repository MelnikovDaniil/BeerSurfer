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
    public GameObject lootboxMenuGameObject;

    public Text beerText;
    public Text scoreText;

    private void Start()
    {
        SoundManager.PlayMusic("lift");
        UpdateInfo();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
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

    private IEnumerator DelayShowLootBoxMenu()
    {
        shop.HideMenu();
        yield return new WaitForSeconds(switchDelay / 2f);
        cameraManager.SetTarget(lootboxMenuGameObject, 5, 1, Vector3.zero);
        yield return new WaitForSeconds(switchDelay / 2);
        lootBoxManger.OpenMenu();
    }

    private IEnumerator DelayShowShopMenu()
    {
        lootBoxManger.HideMenu();
        yield return new WaitForSeconds(switchDelay / 2);
        cameraManager.SetCameraBasePosition();
        yield return new WaitForSeconds(switchDelay / 2);
        shop.OpenMenu();
    }

    public void UpdateInfo()
    {
        scoreText.text = "record: " + RecordMapper.Get();
        beerText.text = BeerMapper.Get().ToString();
    }
}
