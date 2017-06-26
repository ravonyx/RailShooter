using UnityEngine;

namespace RailShooter.Utils
{
    // This class is used to move UI elements to look at the camera
    public class UIMovement : MonoBehaviour
    {
        [SerializeField] 
		private bool m_LookatCamera = true;    
        [SerializeField] 
		private Transform m_UIElement;
        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;

        //transform of camera
        private Transform m_camera;       
             
        [SerializeField] private bool m_RotateWithCamera;       

        private float m_DistanceFromCamera;                     

        private void Start ()
        {
            m_camera = m_camInputManager.CurrentCamera.transform;
			m_DistanceFromCamera = Vector3.Distance (m_UIElement.position, m_camera.position);
        }

        private void Update()
        {
            if (m_LookatCamera)
            {
                m_UIElement.transform.position = new Vector3(m_camera.position.x, m_camera.position.y + 1.0f, m_camera.position.z) + m_camera.transform.parent.forward * m_DistanceFromCamera;
                m_UIElement.transform.rotation = m_camera.transform.parent.rotation;
            }
        }
    }
}