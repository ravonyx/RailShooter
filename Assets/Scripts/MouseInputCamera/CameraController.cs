using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class CameraController : MonoBehaviour
{
    private int _yMinLimit = -80;
    private int _yMaxLimit = 80;

    private Vector2 _cameraRotation;

    [SerializeField]
    public bool isActive;
    [SerializeField]
    public float sensitivity;

    public Vector3 lookDirection;

    void Start()
    {
        // if (!Application.isEditor)
        lookDirection = Vector3.zero;
        UnityEngine.Cursor.visible = false;
        isActive = true;
        _cameraRotation = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!isActive)
            return;



        float y = -Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float x = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        // _cameraRotation.y += y;
        // _cameraRotation.x += x;
        // _cameraRotation.
        // transform.localRotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0);

        lookDirection += new Vector3(x, y, 0.0f);
        lookDirection.y = ClampAngle(lookDirection.y, _yMinLimit, _yMaxLimit);
        transform.localRotation = Quaternion.Euler(lookDirection.y, lookDirection.x, 0.0f);

        //lookDirection += new Vector3(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);
        //transform.localRotation = Quaternion.Euler(lookDirection.y, lookDirection.x, 0.0f);
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Clamp(angle, min, max);
    }

    float Clamp(float value, float min, float max)
    {
        if (value < min)
            value = min;
        if (value > max)
            value = max;
        return value;
    }
}