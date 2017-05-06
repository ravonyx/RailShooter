using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class CameraController : MonoBehaviour
{
    private int _yMinLimit = -80;
    private int _yMaxLimit = 80;

    [SerializeField]
    public bool isActive;
    [SerializeField]
    public float sensitivity;

    private Vector3 lookDirection;

    void Start()
    {
        lookDirection = Vector3.zero;
        isActive = true;
    }

    void Update()
    {
        if (!isActive)
            return;

        float y = -Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float x = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        lookDirection += new Vector3(y, x, 0.0f);
        lookDirection.x = Clamp(lookDirection.x, _yMinLimit, _yMaxLimit);
        transform.localRotation = Quaternion.Euler(lookDirection.x, lookDirection.y, 0.0f);
    }

    public void Reset()
    {
        lookDirection = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
        lookDirection.x = RemapAngle(lookDirection.x, _yMinLimit, _yMaxLimit);
        isActive = true;
    }

    float RemapAngle(float angle, float min, float max)
    {
        while (angle < min || angle > max)
        {
            if (angle < min)
                angle += 180;
            if (angle > max)
                angle -= 180;
        }
        return angle;
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