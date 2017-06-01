using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;
using UnityEngine.VR;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    public class RailShooterController : MonoBehaviour
    {
        [SerializeField] private SessionData.GameType m_gameType;      

        [SerializeField] private Transform m_start;              
        [SerializeField] private SelectionSlider m_selectionSlider;     

        //camera variables
        private Transform m_camera;                    
        private SelectionRadial m_selectionRadial;     
        private Reticle m_reticle;

        [SerializeField]
        private bool m_tutorial;
        [SerializeField]
        private UIController m_UIController;           // Used to encapsulate the UI.
        [SerializeField]
        private InputWarnings m_inputWarnings;         // Tap warnings need to be on for the intro and outro but off for the game itself.

        [SerializeField]
        private CameraFade m_cameraFade;           // Reference to the script that fades the scene to black.

        //control movement of player
        [SerializeField] private PathWalker m_pathWalker;         

                             
        public bool IsPlaying { get; private set; }

        private IEnumerator Start()
        {
            m_cameraFade = Camera.main.GetComponent<CameraFade>();

            SessionData.SetGameType(m_gameType);

            m_camera = Camera.main.transform;
            m_selectionRadial = m_camera.GetComponent<SelectionRadial>();
            m_reticle = m_camera.GetComponent<Reticle>();
            m_pathWalker = m_camera.GetComponent<PathWalker>();

            m_camera.GetComponent<CameraController>().enabled = true;
            m_camera.GetComponent<HideLockMouse>().enabled = true;

            //loop to all phases
            while (true)
            {
                yield return StartCoroutine (StartPhase ());
                yield return StartCoroutine (PlayPhase ());
                yield return StartCoroutine (EndPhase ());
            }
        }

        private IEnumerator StartPhase ()
        {
            yield return StartCoroutine (m_UIController.ShowIntroUI ());

            m_reticle.Show ();
            m_selectionRadial.Hide ();

            m_inputWarnings.TurnOnDoubleTapWarnings ();
            m_inputWarnings.TurnOnSingleTapWarnings ();
            yield return StartCoroutine (m_selectionSlider.WaitForBarToFill ());

            m_inputWarnings.TurnOffDoubleTapWarnings ();
            m_inputWarnings.TurnOffSingleTapWarnings ();
            yield return StartCoroutine (m_UIController.HideIntroUI ());
        }


        private IEnumerator PlayPhase ()
        {
            // Wait for the UI on the player's gun to fade in.
            yield return StartCoroutine(m_UIController.ShowPlayerUI());

            IsPlaying = true;
            m_reticle.Show ();

            // Reset the score.
            SessionData.Restart ();

            if(m_tutorial)
            {
                for (int i = 0; i < m_pathWalker.StopPoints.Count; i++)
                {
                    yield return StartCoroutine(m_pathWalker.PlayUpdate());
                    if(i == 0)
                    {
                        IsPlaying = false;
                        yield return StartCoroutine(m_UIController.ShowEnemiesUI());
                        yield return StartCoroutine(m_selectionRadial.WaitForSelectionRadialToFill());
                        yield return StartCoroutine(m_UIController.HideEnemiesUI());
                        IsPlaying = true;
                    }
                    else if(i == 1)
                    {
                        IsPlaying = false;
                        yield return StartCoroutine(m_UIController.ShowHealthUI());
                        yield return StartCoroutine(m_selectionRadial.WaitForSelectionRadialToFill());
                        yield return StartCoroutine(m_UIController.HideHealthUI());
                        IsPlaying = true;
                    }
                }

            }
            else
            {
                for (int i = 0; i < m_pathWalker.StopPoints.Count; i++)
                    yield return StartCoroutine(m_pathWalker.PlayUpdate());
            }

            //gun UI fade out
            yield return StartCoroutine(m_UIController.HidePlayerUI());
            IsPlaying = false;
        }


        private IEnumerator EndPhase ()
        {
            m_reticle.Hide ();
            yield return StartCoroutine (m_UIController.ShowOutroUI ());
            
            m_inputWarnings.TurnOnDoubleTapWarnings();
            m_inputWarnings.TurnOnSingleTapWarnings();
            yield return StartCoroutine(m_selectionRadial.WaitForSelectionRadialToFill());
           
            m_inputWarnings.TurnOffDoubleTapWarnings();
            m_inputWarnings.TurnOffSingleTapWarnings();
            yield return StartCoroutine(m_UIController.HideOutroUI());


            //reset transform of camera
            yield return StartCoroutine(m_cameraFade.BeginFadeOut(true));

            m_pathWalker.Reset();

            Debug.Log(m_start.position);
            Debug.Log(m_start.localPosition);

            m_camera.transform.localPosition = m_start.position;
            m_camera.transform.localRotation = m_start.rotation;
            yield return StartCoroutine(m_cameraFade.BeginFadeIn(true));
        }

        public IEnumerator GameOver()
        {
            IsPlaying = false;
            StopCoroutine(PlayPhase());

            m_pathWalker.Walking = false;

            yield return StartCoroutine(EndPhase());
        }
    }
}