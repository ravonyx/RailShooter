using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRStandardAssets.Utils;
using RailShooter.Utils;

namespace RailShooter.Menu
{
    // This script is for loading scenes from the main menu.
    // Each 'button' will be a rendering showing the scene
    // that will be loaded and use the SelectionRadial.
    public class MenuButton : MonoBehaviour
    {
        public event Action<MenuButton> OnButtonSelected;                   // This event is triggered when the selection of the button has finished.

        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;
        [SerializeField] private string m_SceneToLoad;                      // The name of the scene to load.
        private SelectionRadial m_selectionRadial;        
        [SerializeField] private InteractiveItem m_InteractiveItem;       // The interactive item for where the user should click to load the level.

        private CameraFade m_cameraFade;
        private bool m_GazeOver;                                            

        void Start()
        {
            m_cameraFade = m_camInputManager.CurrentCamera.GetComponent<CameraFade>();
            Debug.Log(m_camInputManager.CurrentCamera);
        }

        private void OnEnable ()
        {
            Debug.Log(m_camInputManager.CurrentCamera);
            m_selectionRadial = m_camInputManager.CurrentCamera.GetComponent<SelectionRadial>();
            m_InteractiveItem.OnOver += HandleOver;
            m_InteractiveItem.OnOut += HandleOut;
            m_selectionRadial.OnSelectionComplete += HandleSelectionComplete;
        }


        private void OnDisable ()
        {
            m_InteractiveItem.OnOver -= HandleOver;
            m_InteractiveItem.OnOut -= HandleOut;
            m_selectionRadial.OnSelectionComplete -= HandleSelectionComplete;
        }


        private void HandleOver()
        {
            // When the user looks at the rendering of the scene, show the radial.
            m_selectionRadial.Show();
            m_GazeOver = true;
        }


        private void HandleOut()
        {
            // When the user looks away from the rendering of the scene, hide the radial.
            m_selectionRadial.Hide();
            m_GazeOver = false;
        }


        private void HandleSelectionComplete()
        {
            // If the user is looking at the rendering of the scene when the radial's selection finishes, activate the button.
            if(m_GazeOver)
                StartCoroutine (ActivateButton());
        }

        private IEnumerator ActivateButton()
        {
            // If the camera is already fading, ignore.
            if (m_cameraFade.IsFading)
                yield break;

            // If anything is subscribed to the OnButtonSelected event, call it.
            if (OnButtonSelected != null)
                OnButtonSelected(this);

            // Wait for the camera to fade out.
            yield return StartCoroutine(m_cameraFade.BeginFadeOut(true));

            // Load the level.
            SceneManager.LoadScene(m_SceneToLoad, LoadSceneMode.Single);
        }
    }
}