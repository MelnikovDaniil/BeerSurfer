using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject _target;
    public Camera camera;
    private float secondsForMoving;
    private float toCameraSize;
    private float currentTime;
    private const float StandartCameraSize = 5;
    private const float XCameraKoof = 8.7f;
    private const float YCameraKoof = 5f;
    private Vector3 currentPosition;
    private float currentCameraSize;
    private Vector3 cameraShift;


    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {

            if (secondsForMoving > currentTime)
            {
                currentTime += Time.deltaTime;
                var k = currentTime / secondsForMoving;
                var cameraSize = Mathf.Lerp(currentCameraSize, toCameraSize, k);
                var cameraPosition = Vector3.Lerp(currentPosition, _target.transform.position + cameraShift, k);
                transform.position = cameraPosition;
                camera.orthographicSize = cameraSize;
            }
            else
            {
                transform.position = _target.transform.position;
                _target = null;
            }
        }

    }

    public void SetTarget(GameObject target, float size, float time, Vector3 cameraShift)
    {
        currentPosition = transform.position;
        currentCameraSize = camera.orthographicSize;
        secondsForMoving = time;
        _target = target;
        this.cameraShift = cameraShift;
        toCameraSize = size;
        currentTime = 0;
    }

    public void SetCameraBasePosition(float time = 1)
    {
        currentPosition = transform.position;
        _target = new GameObject("basePosition");
        _target.transform.position = new Vector3(0, 0, -10);
        currentCameraSize = camera.orthographicSize;
        secondsForMoving = time;
        this.cameraShift = Vector3.zero;
        toCameraSize = StandartCameraSize;
        currentTime = 0;
    }
}
