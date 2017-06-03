using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using RailShooter.Utils;

namespace RailShooter.Utils
{
    public class ReturnToMainMenu : MonoBehaviour
    {
        [SerializeField] private string m_menuSceneName = "MainMenu";   
        [SerializeField] private CameraFade m_cameraFade;

        private Inputs m_inputs;
        private void OnEnable ()
        {
            m_inputs = GetComponent<Inputs>();
            m_inputs.OnCancel += HandleCancel;
            m_cameraFade = GetComponent<CameraFade>();
        }

        private void OnDisable ()
        {
            m_inputs.OnCancel += HandleCancel;
        }

        private void HandleCancel ()
        {
            StartCoroutine (FadeToMenu ());
        }

        private IEnumerator FadeToMenu ()
        {
            yield return StartCoroutine (m_cameraFade.BeginFadeOut (true));
            SceneManager.LoadScene(m_menuSceneName, LoadSceneMode.Single);
        }
    }
}