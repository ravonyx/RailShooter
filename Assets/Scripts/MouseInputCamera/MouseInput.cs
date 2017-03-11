using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // This class encapsulates all the input required for most VR games.
    // It has events that can be subscribed to by classes that need specific input.
    // This class must exist in every scene and so can be attached to the main
    // camera for ease.
    public class MouseInput : MonoBehaviour
    {
        public event Action OnClick;                                // Called when Fire1 is released and it's not a double click.
        public event Action OnDown;                                 // Called when Fire1 is pressed.
        public event Action OnUp;                                   // Called when Fire1 is released.
        public event Action OnDoubleClick;                          // Called when a double click is detected.
        public event Action OnCancel;                               // Called when Cancel is pressed.


        [SerializeField]
        private float m_DoubleClickTime = 0.3f;    //The max time allowed between double clicks


        private Vector2 m_MouseDownPosition;                        // The screen position of the mouse when Fire1 is pressed.
        private Vector2 m_MouseUpPosition;                          // The screen position of the mouse when Fire1 is released.
        private float m_LastMouseUpTime;                            // The time when Fire1 was last released.
        private float m_LastHorizontalValue;                        // The previous value of the horizontal axis used to detect keyboard swipes.
        private float m_LastVerticalValue;                          // The previous value of the vertical axis used to detect keyboard swipes.


        public float DoubleClickTime { get { return m_DoubleClickTime; } }


        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            // Set the default swipe to be none.
            if (Input.GetButtonDown("Fire1"))
            {
                // When Fire1 is pressed record the position of the mouse.
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                // If anything has subscribed to OnDown call it.
                if (OnDown != null)
                    OnDown();
            }

            // This if statement is to gather information about the mouse when the button is up.
            if (Input.GetButtonUp("Fire1"))
            {
                // When Fire1 is released record the position of the mouse.
                m_MouseUpPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                // Detect the direction between the mouse positions when Fire1 is pressed and released.
            }


            // This if statement is to trigger events based on the information gathered before.
            if (Input.GetButtonUp("Fire1"))
            {
                // If anything has subscribed to OnUp call it.
                if (OnUp != null)
                    OnUp();

                // If the time between the last release of Fire1 and now is less
                // than the allowed double click time then it's a double click.
                if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                {
                    // If anything has subscribed to OnDoubleClick call it.
                    if (OnDoubleClick != null)
                        OnDoubleClick();
                }
                else
                {
                    // If it's not a double click, it's a single click.
                    // If anything has subscribed to OnClick call it.
                    if (OnClick != null)
                        OnClick();
                }

                // Record the time when Fire1 is released.
                m_LastMouseUpTime = Time.time;
            }

            // If the Cancel button is pressed and there are subscribers to OnCancel call it.
            if (Input.GetButtonDown("Cancel"))
            {
                if (OnCancel != null)
                    OnCancel();
            }
        }

        private void OnDestroy()
        {
            // Ensure that all events are unsubscribed when this is destroyed.
            OnClick = null;
            OnDoubleClick = null;
            OnDown = null;
            OnUp = null;
        }
    }
}