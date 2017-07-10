using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        bool m_foveActive = false;


        [SerializeField]
        private GameObject m_OneArmWeapon;
        [SerializeField]
        private GameObject[] m_TwoArmsWeapon;

        [SerializeField]
        private bool m_menu;

        private Camera m_currentCamera;
        public Camera CurrentCamera
        {
            get
            {
                return m_currentCamera;
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

        private Inputs m_currentInputs;
        public Inputs CurrentInputs
        {
            get
            {
                return m_currentInputs;
            }
        }

        void OnEnable()
        {
            m_currentInputs = GetComponent<Inputs>();
            //TODO : add fove camera
            //set the current camera
            if(m_foveActive)
            {
                m_currentCamera = m_foveCam;
                m_VRCam.transform.parent.gameObject.SetActive(false);
                m_mouseCam.transform.parent.gameObject.SetActive(false);
            }

            else if (VRSettings.enabled)
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

            if (m_menu)
                return;

			//set the current input
			OVRInput.Update();
			OVRInput.Controller controller = OVRInput.GetConnectedControllers();
			Debug.Log("Controller selected = " + controller);

			m_currentInputName = controller.ToString();
			if (m_currentInputName == "None")
				m_currentInputName = "Mouse";
			if (controller == OVRInput.Controller.Touch)
			{
				for(int i = 0; i < m_TwoArmsWeapon.Length; i++)
					m_TwoArmsWeapon[i].SetActive(true);
				m_OneArmWeapon.SetActive(false);

                m_currentCamera.GetComponent<Raycaster>().enabled = false;
                m_currentCamera.GetComponent<Reticle>().enabled = false;
                m_currentCamera.GetComponent<SelectionRadial>().enabled = false;
            }

			else
			{
				for (int i = 0; i < m_TwoArmsWeapon.Length; i++)
					m_TwoArmsWeapon[i].SetActive(false);
				m_OneArmWeapon.SetActive(true);
			}
        }

        public Transform GetCurrentCameraRoot()
        {
            return m_currentCamera.transform.parent;
        }
    }
}