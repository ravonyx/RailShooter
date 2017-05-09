using UnityEngine;
using System.Collections;
using Assets.RailShooter;

namespace RailShooter.Utils
{
    public class HideLockMouse : MonoBehaviour
    {
        private bool m_lockCursor = true;
        private PathWalker m_pathWalker;
        [SerializeField]
        private RailShooterController m_railShooterController;

        void Start()
        {
            m_pathWalker = GetComponent<PathWalker>();
        }

        void Update()
        {
            // pressing esc toggles between hide/show
            if (Input.GetKeyDown(KeyCode.Space))
                m_lockCursor = !m_lockCursor;

            Cursor.lockState = m_lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            GetComponent<CameraController>().enabled = m_lockCursor;

            if (m_lockCursor == false)
                m_pathWalker.Walking = false;
            else if (m_railShooterController.IsPlaying)
                m_pathWalker.Walking = true;
            Cursor.visible = !m_lockCursor;
        }
    }
}