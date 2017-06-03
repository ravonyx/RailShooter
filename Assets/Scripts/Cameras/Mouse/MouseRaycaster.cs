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
        private LayerMask m_ExclusionLayers;           
        [SerializeField]
        private bool m_ShowDebugRay;                   
        [SerializeField]
        private float m_RayLength = 500f;              // How far into the scene the ray is cast.

        private Inputs m_inputs;
        private InteractiveItem m_currentInteractible;                
        private InteractiveItem m_lastInteractible;
        private Reticle m_Reticle;

        // Utility for other classes to get the current interactive item
        public InteractiveItem CurrentInteractible
        {
            get { return m_currentInteractible; }
        }

        private void OnEnable()
        {
            m_inputs = GetComponent<Inputs>();

            m_inputs.OnClick += HandleClick;
            m_inputs.OnDoubleClick += HandleDoubleClick;
            m_inputs.OnUp += HandleUp;
            m_inputs.OnDown += HandleDown;
        }


        private void OnDisable()
        {
            m_inputs.OnClick -= HandleClick;
            m_inputs.OnDoubleClick -= HandleDoubleClick;
            m_inputs.OnUp -= HandleUp;
            m_inputs.OnDown -= HandleDown;
        }

        void Start()
        {
            m_Reticle = GetComponent<Reticle>();
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
                    Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue, 1.0f);
                ray = new Ray(transform.position, transform.forward);
            //}

            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
            {
                InteractiveItem interactible = hit.collider.GetComponent<InteractiveItem>(); //attempt to get the InteractiveItem on the hit object
                m_currentInteractible = interactible;
                if (interactible && interactible != m_lastInteractible)
                    interactible.Over();
            
                if (interactible != m_lastInteractible)
                    DeactiveLastInteractible();

                m_lastInteractible = interactible;
            
                if (m_Reticle)
                    m_Reticle.SetPosition(hit);

            }
            else
            {
                DeactiveLastInteractible();
                m_currentInteractible = null;

                if (m_Reticle)
                    m_Reticle.SetPosition();
            }
        }
        private void DeactiveLastInteractible()
        {
            if (m_lastInteractible == null)
                return;

            m_lastInteractible.Out();
            m_lastInteractible = null;
        }


        private void HandleUp()
        {
            if (m_currentInteractible != null)
                m_currentInteractible.Up();
        }


        private void HandleDown()
        {
            if (m_currentInteractible != null)
                m_currentInteractible.Down();
        }


        private void HandleClick()
        {
            if (m_currentInteractible != null)
                m_currentInteractible.Click();
        }


        private void HandleDoubleClick()
        {
            if (m_currentInteractible != null)
                m_currentInteractible.DoubleClick();

        }
    }

}
