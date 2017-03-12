using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using RailShooter.Utils;

namespace VRStandardAssets.Utils
{
    // This class simply allows the user to return to the main menu.
    public class ReturnToMainMenu : MonoBehaviour
    {
        [SerializeField] private string m_MenuSceneName = "MainMenu";   // The name of the main menu scene.
        [SerializeField] private VRInput m_VRInput;                     // Reference to the VRInput in order to know when Cancel is pressed.
        [SerializeField] private MouseInput m_mouseInput;                     // Reference to the VRInput in order to know when Cancel is pressed.
        [SerializeField] private CameraFade m_CameraFade;           // Reference to the script that fades the scene to black.


        private void OnEnable ()
        {
            if (VRSettings.enabled == false)
                m_mouseInput.OnCancel += HandleCancel;
            else
                m_VRInput.OnCancel += HandleCancel;

            m_CameraFade = Camera.main.GetComponent<CameraFade>();
        }


        private void OnDisable ()
        {
            if (VRSettings.enabled == false)
                m_mouseInput.OnCancel += HandleCancel;
            else
                m_VRInput.OnCancel -= HandleCancel;
        }


        private void HandleCancel ()
        {
            StartCoroutine (FadeToMenu ());
        }


        private IEnumerator FadeToMenu ()
        {
            // Wait for the screen to fade out.
            yield return StartCoroutine (m_CameraFade.BeginFadeOut (true));

            // Load the main menu by itself.
            SceneManager.LoadScene(m_MenuSceneName, LoadSceneMode.Single);
        }
    }
}