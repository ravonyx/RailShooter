using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;

namespace VRStandardAssets.Utils
{
    public class MouseRaycaster : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Camera;
        [SerializeField]
        private LayerMask m_ExclusionLayers;           
        [SerializeField]
        private Reticle m_Reticle;                    
        [SerializeField]
        private MouseInput m_mouseInput;                   
        [SerializeField]
        private bool m_ShowDebugRay;                   
        [SerializeField]
        private float m_RayLength = 500f;              // How far into the scene the ray is cast.


        private InteractiveItem m_CurrentInteractible;                
        private InteractiveItem m_LastInteractible;                   


        // Utility for other classes to get the current interactive item
        public InteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        private void OnEnable()
        {
            m_mouseInput.OnClick += HandleClick;
            m_mouseInput.OnDoubleClick += HandleDoubleClick;
            m_mouseInput.OnUp += HandleUp;
            m_mouseInput.OnDown += HandleDown;
        }


        private void OnDisable()
        {
            m_mouseInput.OnClick -= HandleClick;
            m_mouseInput.OnDoubleClick -= HandleDoubleClick;
            m_mouseInput.OnUp -= HandleUp;
            m_mouseInput.OnDown -= HandleDown;
        }

        void Start()
        {
            Cursor.visible = false;
        }

        void Update()
        {
            // Show the debug ray if required
            if (m_ShowDebugRay)
            {
                Debug.DrawRay(m_Camera.position, m_Camera.forward * 5.0f, Color.blue, 1.0f);
            }

            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
            {
                InteractiveItem interactible = hit.collider.GetComponent<InteractiveItem>(); //attempt to get the InteractiveItem on the hit object
                m_CurrentInteractible = interactible;

                // If we hit an interactive item and it's not the same as the last interactive item, then call Over
                if (interactible && interactible != m_LastInteractible)
                    interactible.Over();

                // Deactive the last interactive item 
                if (interactible != m_LastInteractible)
                    DeactiveLastInteractible();

                m_LastInteractible = interactible;

                // Something was hit, set at the hit position.
                if (m_Reticle)
                    m_Reticle.SetPosition(hit);

                /* if (OnRaycasthit != null)
                     OnRaycasthit(hit);*/
            }
            else
            {
                // Nothing was hit, deactive the last interactive item.
                DeactiveLastInteractible();
                m_CurrentInteractible = null;

                // Position the reticle at default distance.
                if (m_Reticle)
                    m_Reticle.SetPosition();
            }
        }
        private void DeactiveLastInteractible()
        {
            if (m_LastInteractible == null)
                return;

            m_LastInteractible.Out();
            m_LastInteractible = null;
        }


        private void HandleUp()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Up();
        }


        private void HandleDown()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Down();
        }


        private void HandleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Click();
        }


        private void HandleDoubleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.DoubleClick();

        }
    }

}
