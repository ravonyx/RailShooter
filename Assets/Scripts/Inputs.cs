using System;
using UnityEngine;

namespace RailShooter.Utils
{
    public class Inputs : MonoBehaviour
    {
        public event Action OnClick;                                
        public event Action OnDown;                                 
        public event Action OnDownLeft;

        public event Action OnUp;                                  
        public event Action OnDoubleClick;                          
        public event Action OnCancel;                               

        [SerializeField]
        private float m_DoubleClickTime = 0.3f;    
        private float m_LastMouseUpTime;                            

        public float DoubleClickTime { get { return m_DoubleClickTime; } }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Fire1");
                if (OnDown != null)
                    OnDown();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Debug.Log("Fire2");
                if (OnDownLeft != null)
                    OnDownLeft();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (OnUp != null)
                    OnUp();

                if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                {
                    if (OnDoubleClick != null)
                        OnDoubleClick();
                }
                else
                {
                    if (OnClick != null)
                        OnClick();
                }

                m_LastMouseUpTime = Time.time;
            }

            if (Input.GetButtonDown("Cancel"))
            {
                if (OnCancel != null)
                    OnCancel();
            }
        }

        private void OnDestroy()
        {
            OnClick = null;
            OnDoubleClick = null;
            OnDown = null;
            OnUp = null;
        }
    }
}