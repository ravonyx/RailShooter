using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RailShooter.Utils;
using VRStandardAssets.Common;

namespace Assets.RailShooter
{
    class Raycaster : MonoBehaviour
    {

        [SerializeField]
        private LayerMask m_exclusionLayers;
        [SerializeField]
        private bool m_showDebugRay;
        private float m_rayLength = 500f;              // How far into the scene the ray is cast.

        private Inputs m_inputs;
        private InteractiveItem m_currentInteractible;
        private InteractiveItem m_lastInteractible;
        private Reticle m_reticle;

        [SerializeField]
        private PKFxFX m_impactParticles;
        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;

        public Vector3 m_LastHitPoint
        {
            get
            {
                return m_lastHitPoint;
            }
        }
        private Vector3 m_lastHitPoint;

        // Utility for other classes to get the current interactive item
        public InteractiveItem CurrentInteractible
        {
            get { return m_currentInteractible; }
        }

        private void OnEnable()
        {
            m_inputs = m_camInputManager.CurrentInputs;

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
            m_reticle = GetComponent<Reticle>();
            Cursor.visible = false;
        }

        void Update()
        {
            Ray ray = new Ray();
            RaycastHit hit;

            if (m_showDebugRay)
                Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue, 1.0f);
            ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(ray, out hit, m_rayLength, ~m_exclusionLayers))
            {
                InteractiveItem interactible = hit.collider.GetComponent<InteractiveItem>(); //attempt to get the InteractiveItem on the hit object
                m_currentInteractible = interactible;

                m_lastHitPoint = hit.point;
                m_impactParticles.transform.position = m_LastHitPoint;

                if (interactible && interactible != m_lastInteractible)
                    interactible.Over();

                if (interactible != m_lastInteractible)
                    DeactiveLastInteractible();

                m_lastInteractible = interactible;

                if (m_reticle)
                    m_reticle.SetPosition(hit);

            }
            else
            {
                DeactiveLastInteractible();
                m_currentInteractible = null;

                if (m_reticle)
                    m_reticle.SetPosition();
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
