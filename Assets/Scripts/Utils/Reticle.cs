using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
namespace VRStandardAssets.Utils
{
    // The reticle is a small point at the centre of the screen.
    // It is used as a visual aid for aiming. The position of the
    // reticle is either at a default position in space or on the
    // surface of a InteractiveItem as determined by the VREyeRaycaster.
    public class Reticle : MonoBehaviour
    {
        [SerializeField] private float m_DefaultDistance = 5f;      // The default distance away from the camera the reticle is placed.
        [SerializeField] private bool m_UseNormal;                  // Whether the reticle should be placed parallel to a surface.
        [SerializeField] private Image m_Image;                     // Reference to the image component that represents the reticle.
        [SerializeField] private Transform m_ReticleTransform;      // We need to affect the reticle's transform.
        [SerializeField] private Transform m_Camera;                // The reticle is always placed relative to the camera.


        private Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.
        private Quaternion m_OriginalRotation;                      // Used to store the original rotation of the reticle.

        public bool UseNormal
        {
            get { return m_UseNormal; }
            set { m_UseNormal = value; }
        }

        public Transform ReticleTransform { get { return m_ReticleTransform; } }

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
            m_ReticleTransform.position = m_Camera.position + m_Camera.forward * m_DefaultDistance;
            m_ReticleTransform.localScale = m_OriginalScale * m_DefaultDistance;
            m_ReticleTransform.localRotation = m_OriginalRotation;
        }
        public void SetPosition2D()
        {
            var mousePos = Input.mousePosition;
            mousePos.z = m_DefaultDistance;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            m_ReticleTransform.position = mousePos + ray.direction * m_DefaultDistance;
            m_ReticleTransform.localScale = m_OriginalScale * m_DefaultDistance * 2;
            m_ReticleTransform.localRotation = m_OriginalRotation;
        }

        public void SetPosition (RaycastHit hit)
        {
            m_ReticleTransform.position = hit.point;
            m_ReticleTransform.localScale = m_OriginalScale * hit.distance;
            
            if (m_UseNormal)
                m_ReticleTransform.rotation = Quaternion.FromToRotation (Vector3.forward, hit.normal);
            else
                m_ReticleTransform.localRotation = m_OriginalRotation;
        }
    }
}