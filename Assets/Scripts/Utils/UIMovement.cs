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
        [SerializeField] private float m_FollowSpeed = 10f;     

        private float m_DistanceFromCamera;                     

        private void Start ()
        {
            m_camera = m_camInputManager.CurrentCamera.transform;
			m_DistanceFromCamera = Vector3.Distance (m_UIElement.position, m_camera.position);
        }

        private void Update()
        {
			m_UIElement.transform.parent.position = new Vector3(m_UIElement.transform.parent.position.x, m_camera.position.y, m_UIElement.transform.parent.position.z);
            if(m_LookatCamera)
				m_UIElement.rotation = Quaternion.LookRotation(m_UIElement.position - m_camera.position);

            if (m_RotateWithCamera)
            {
				Vector3 targetDirection = Vector3.ProjectOnPlane (m_camera.forward, Vector3.up).normalized;
				Vector3 targetPosition = m_camera.position + targetDirection * m_DistanceFromCamera;
                targetPosition = Vector3.Lerp(m_UIElement.position, targetPosition, m_FollowSpeed * Time.deltaTime);
                targetPosition.y = m_UIElement.position.y;
                m_UIElement.position = targetPosition;
            }
        }
    }
}