using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.VR;

namespace RailShooter.Utils
{
    public class CamerasAndInputsManager : MonoBehaviour
    {

        [SerializeField]
        private Camera m_VRCam;
        [SerializeField]
        private Camera m_mouseCam;
        [SerializeField]
        private Camera m_foveCam;
        private Camera m_currentCamera;
        public Camera CurrentCamera
        {
            get
            {
                return m_currentCamera;
            }
        }

        void Awake()
        {
            if (VRSettings.enabled)
                m_currentCamera = m_VRCam;
            else
                m_currentCamera = m_mouseCam;

            Debug.Log("Selected Camera = "  + m_currentCamera.name);
        }

        public Transform GetCurrentCameraRoot()
        {
            return m_currentCamera.transform.parent;
        }
        public Inputs GetCurrentInputs()
        {
            return m_currentCamera.GetComponent<Inputs>();
        }
    }
}