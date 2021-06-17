using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Space(20)]
    public CameraManager cameraManager;
    public float switchDelay = 1f;

    public Shop shop;
    public LootBoxManger lootBoxManger;
    public GameObject lootboxMenuGameObject;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowLootBoxMenu()
    {
        StartCoroutine(DelayShowLootBoxMenu());
    }

    public void ShowShopMenu()
    {
        StartCoroutine(DelayShowShopMenu());
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
}
