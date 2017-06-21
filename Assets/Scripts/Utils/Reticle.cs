using UnityEngine;
using UnityEngine.UI;

namespace RailShooter.Utils
{
    public class Reticle : MonoBehaviour
    {
        [SerializeField]
        private Image m_Image;                     // Reference to the image component that represents the reticle.
     

        private Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.
        private Quaternion m_OriginalRotation;                      // Used to store the original rotation of the reticle.

        [SerializeField]
        private Transform m_ReticleTransform;      // We need to affect the reticle's transform.
        public Transform ReticleTransform
        {
            get
            {
                return m_ReticleTransform;
            }
        }

        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;


        private void Awake()
        {
            m_OriginalScale = m_ReticleTransform.localScale;
            m_OriginalRotation = m_ReticleTransform.localRotation;
        }

        public void Hide()
        {
            m_Image.enabled = false;
        }
        public void Show()
        {
            m_Image.enabled = true;
        }

        public void SetPosition ()
        {
            Camera camera = m_camInputManager.CurrentCamera;
            float distance = camera.farClipPlane * 0.95f;

            Debug.Log("distance = " + distance);
            m_ReticleTransform.position = transform.position + (transform.forward * distance);
            m_ReticleTransform.localScale = m_OriginalScale * distance;
            m_ReticleTransform.localRotation = m_OriginalRotation;
        }

        public void SetPosition (RaycastHit hit)
        {
            float distance = hit.distance;
            m_ReticleTransform.localScale = m_OriginalScale * hit.distance;
            m_ReticleTransform.localRotation = m_OriginalRotation;
            m_ReticleTransform.position = transform.position + (transform.forward * distance);
        }
    }
}