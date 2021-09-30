using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenshotManger : MonoBehaviour
{
    public static ScreenshotManger Instance;

    private Camera camera;

    private void Awake()
    {
        Instance = this;
        camera = GetComponent<Camera>();
    }

    public static Sprite TakeScreenshot(int width, int height)
    {
        Instance.StartCoroutine(Instance.WhaitForEndOfFrame());

        var screenshot = new Texture2D(width, height);
        var renderTexture = RenderTexture.GetTemporary(width, height, 16);
        Instance.camera.targetTexture = renderTexture;
        Instance.camera.orthographicSize /= 2;
        Instance.camera.Render();
        RenderTexture.active = renderTexture;
        var rect = new Rect(0, 0, width, height);
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();
        Instance.camera.orthographicSize *= 2;
        Instance.camera.targetTexture = null;
        RenderTexture.active = null;
        var spriteResult = Sprite.Create(screenshot, rect, new Vector2(0.5f, 0.5f));
        return spriteResult;
    }
    private IEnumerator WhaitForEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        
    }
}
