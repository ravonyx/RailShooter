using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_leftHandAnchor;
    [SerializeField]
    private Transform m_rightHandAnchor;

    // Class to suppress ? / need to test
 //   void Start ()
 //   {
 //       UpdateAnchors();
 //   }
 //
 //   void Update ()
 //   {
 //       OVRInput.Update();
 //       UpdateAnchors();
 //   }
 //
 //   private void UpdateAnchors()
 //   {
 //       m_leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
 //       m_rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
 //
 //       m_leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
 //       m_rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
 //   }
}
