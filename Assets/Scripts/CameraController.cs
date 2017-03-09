using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class CameraController : MonoBehaviour
{
    private int _yMinLimit = -40;
    private int _yMaxLimit = 50;

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
        lookDirection += new Vector3(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0.0f);
        transform.localRotation = Quaternion.Euler(lookDirection.y, lookDirection.x, 0.0f);
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