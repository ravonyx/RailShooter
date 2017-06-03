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
            //TODO : add fove camera
            if (VRSettings.enabled)
            {
                m_currentCamera = m_VRCam;
                m_mouseCam.transform.parent.gameObject.SetActive(false);
                m_foveCam.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                m_currentCamera = m_mouseCam;
                m_VRCam.transform.parent.gameObject.SetActive(false);
                m_foveCam.transform.parent.gameObject.SetActive(false);
            }

            m_currentCamera.transform.parent.gameObject.SetActive(true);
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