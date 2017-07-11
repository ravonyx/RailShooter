using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RailShooter.Utils;

public class MovePlatform : MonoBehaviour
{
    [SerializeField]
    CamerasAndInputsManager m_camInputManager;
    [SerializeField]
    float m_deltaY;

    private void LateUpdate()
    {
        Quaternion quat = Quaternion.Euler(180, 0, 0);
        Quaternion newRotation = m_camInputManager.CurrentCamera.transform.parent.transform.rotation * quat;

        transform.rotation = newRotation;

        Vector3 camPos = m_camInputManager.CurrentCamera.transform.position;
        Vector3 newPosition = m_camInputManager.CurrentCamera.transform.parent.transform.position + m_camInputManager.CurrentCamera.transform.parent.transform.up * -m_deltaY;

        transform.position = newPosition;
    }
}
