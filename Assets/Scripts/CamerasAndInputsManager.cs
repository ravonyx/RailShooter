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


        [SerializeField]
        private GameObject m_OneArmWeapon;
        [SerializeField]
        private GameObject[] m_TwoArmsWeapon;

        private Camera m_currentCamera;
        public Camera CurrentCamera
        {
            get
            {
                return m_currentCamera;
            }
        }

        private GameObject[] m_currentInputs;
        public GameObject[] CurrentInputs
        {
            get
            {
                return m_currentInputs;
            }
        }

        private string m_currentInputName;
        public string CurrentInputName
        {
            get
            {
                return m_currentInputName;
            }
        }

        void Awake()
        {
            //TODO : add fove camera
            //set the current camera
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

			//test in awake

			//set the current input
			OVRInput.Update();
			OVRInput.Controller controller = OVRInput.GetConnectedControllers();
			Debug.Log("Controller selected = " + controller);

			m_currentInputName = controller.ToString();
			if (m_currentInputName == "None")
				m_currentInputName = "Mouse";
			if (controller == OVRInput.Controller.Touch)
			{
				m_currentInputs = m_TwoArmsWeapon;
				for(int i = 0; i < m_TwoArmsWeapon.Length; i++)
					m_TwoArmsWeapon[i].SetActive(true);
				m_OneArmWeapon.SetActive(false);
			}

			else
			{
				//m_currentInputs[0] = m_OneArmWeapon;
				for (int i = 0; i < m_TwoArmsWeapon.Length; i++)
					m_TwoArmsWeapon[i].SetActive(false);
				m_OneArmWeapon.SetActive(true);
			}
        }

        public Transform GetCurrentCameraRoot()
        {
            return m_currentCamera.transform.parent;
        }
        public Inputs GetCurrentInputs()
        {
            return m_currentCamera.GetComponent<Inputs>();
        }

        /*void Update()
        {
            //set the current input
            OVRInput.Update();
            OVRInput.Controller controller = OVRInput.GetConnectedControllers();
            Debug.Log(controller);
            m_currentInputName = controller.ToString();
            if (m_currentInputName == "None")
                m_currentInputName = "Mouse";
            if (controller == OVRInput.Controller.Touch)
            {
                m_currentInputs = m_TwoArmsWeapon;
                for(int i = 0; i < m_TwoArmsWeapon.Length; i++)
                    m_TwoArmsWeapon[i].SetActive(true);
                m_OneArmWeapon.SetActive(false);
            }

            else
            {
                //m_currentInputs[0] = m_OneArmWeapon;
                for (int i = 0; i < m_TwoArmsWeapon.Length; i++)
                    m_TwoArmsWeapon[i].SetActive(false);
                m_OneArmWeapon.SetActive(true);
            }
        }*/
    }
}