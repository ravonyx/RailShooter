using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class CameraController : MonoBehaviour
{
    private int _yMinLimit = -80;
    private int _yMaxLimit = 80;

    [SerializeField]
    private bool m_isActive;
    [SerializeField]
    private float m_sensitivity;

    private Vector3 m_lookDirection;

    void Start()
    {
        m_lookDirection = Vector3.zero;
        m_isActive = true;
    }

    void Update()
    {
        if (!m_isActive)
            return;

        float y = -Input.GetAxis("Mouse Y") * Time.deltaTime * m_sensitivity;
        float x = Input.GetAxis("Mouse X") * Time.deltaTime * m_sensitivity;

        Debug.Log("x " + x + "y " + y);
        m_lookDirection += new Vector3(y, x, 0.0f);
        m_lookDirection.x = Clamp(m_lookDirection.x, _yMinLimit, _yMaxLimit);
        transform.localRotation = Quaternion.Euler(m_lookDirection.x, m_lookDirection.y, 0.0f);
    }

    public void Reset()
    {
        m_lookDirection = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
        m_lookDirection.x = RemapAngle(m_lookDirection.x, _yMinLimit, _yMaxLimit);
        m_isActive = true;
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