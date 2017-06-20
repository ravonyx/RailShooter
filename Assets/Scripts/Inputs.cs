using System;
using UnityEngine;

namespace RailShooter.Utils
{
    public class Inputs : MonoBehaviour
    {
        public enum SwipeDirection
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        };

        public event Action<SwipeDirection> OnSwipe;               
        public event Action OnClick;                                
        public event Action OnDown;                                 
        public event Action OnUp;                                  
        public event Action OnDoubleClick;                          
        public event Action OnCancel;                               

        [SerializeField]
        private float m_DoubleClickTime = 0.3f;    
        [SerializeField]
        private float m_SwipeWidth = 0.3f;         

        private Vector2 m_MouseDownPosition;                        
        private Vector2 m_MouseUpPosition;                          
        private float m_LastMouseUpTime;                            
        private float m_LastHorizontalValue;                       
        private float m_LastVerticalValue;                          

        public float DoubleClickTime { get { return m_DoubleClickTime; } }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            SwipeDirection swipe = SwipeDirection.NONE;

            if (Input.GetButtonDown("Fire1"))
            {
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                Debug.Log("Fire1");
                if (OnDown != null)
                    OnDown();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Debug.Log("Fire2");
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                if (OnDown != null)
                    OnDown();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                m_MouseUpPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                swipe = DetectSwipe();
            }

            if (swipe == SwipeDirection.NONE)
                swipe = DetectKeyboardEmulatedSwipe();

            if (OnSwipe != null)
                OnSwipe(swipe);

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


        private SwipeDirection DetectSwipe()
        {
            Vector2 swipeData = (m_MouseUpPosition - m_MouseDownPosition).normalized;
            bool swipeIsVertical = Mathf.Abs(swipeData.x) < m_SwipeWidth;
            bool swipeIsHorizontal = Mathf.Abs(swipeData.y) < m_SwipeWidth;
            if (swipeData.y > 0f && swipeIsVertical)
                return SwipeDirection.UP;
            if (swipeData.y < 0f && swipeIsVertical)
                return SwipeDirection.DOWN;
            if (swipeData.x > 0f && swipeIsHorizontal)
                return SwipeDirection.RIGHT;
            if (swipeData.x < 0f && swipeIsHorizontal)
                return SwipeDirection.LEFT;
            return SwipeDirection.NONE;
        }


        private SwipeDirection DetectKeyboardEmulatedSwipe()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool noHorizontalInputPreviously = Mathf.Abs(m_LastHorizontalValue) < float.Epsilon;
            bool noVerticalInputPreviously = Mathf.Abs(m_LastVerticalValue) < float.Epsilon;

            m_LastHorizontalValue = horizontal;
            m_LastVerticalValue = vertical;

            if (vertical > 0f && noVerticalInputPreviously)
                return SwipeDirection.UP;

            if (vertical < 0f && noVerticalInputPreviously)
                return SwipeDirection.DOWN;

            if (horizontal > 0f && noHorizontalInputPreviously)
                return SwipeDirection.RIGHT;

            if (horizontal < 0f && noHorizontalInputPreviously)
                return SwipeDirection.LEFT;

            return SwipeDirection.NONE;
        }


        private void OnDestroy()
        {
            OnSwipe = null;
            OnClick = null;
            OnDoubleClick = null;
            OnDown = null;
            OnUp = null;
        }
    }
}