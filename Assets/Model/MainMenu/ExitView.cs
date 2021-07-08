using UnityEngine;

public class ExitView : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Application quiet");
        Application.Quit();
    }
}
