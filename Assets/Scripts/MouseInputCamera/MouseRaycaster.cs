using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;
using VRStandardAssets.Common;

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
            Ray ray = new Ray();
            RaycastHit hit;

         /*   if (SessionData.GetGameType() == SessionData.GameType.SERIOUSSHOOTER)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = 5.0f;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (m_ShowDebugRay)
                    Debug.DrawRay(mousePos, ray.direction * 5.0f, Color.green, 1.0f);
            }
            else
            {*/
                if (m_ShowDebugRay)
                    Debug.DrawRay(m_Camera.position, m_Camera.forward * 5.0f, Color.blue, 1.0f);
                ray = new Ray(m_Camera.position, m_Camera.forward);
            //}

            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
            {
                InteractiveItem interactible = hit.collider.GetComponent<InteractiveItem>(); //attempt to get the InteractiveItem on the hit object
                m_CurrentInteractible = interactible;
                if (interactible && interactible != m_LastInteractible)
                    interactible.Over();
            
                if (interactible != m_LastInteractible)
                    DeactiveLastInteractible();
            
                m_LastInteractible = interactible;
            
                if (m_Reticle)
                    m_Reticle.SetPosition(hit);

            }
            else
            {
                DeactiveLastInteractible();
                m_CurrentInteractible = null;

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
