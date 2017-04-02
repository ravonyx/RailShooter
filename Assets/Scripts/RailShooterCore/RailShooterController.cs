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
        [SerializeField] private SessionData.GameType m_GameType;      



        [SerializeField] private Transform m_Start;              
        [SerializeField] private SelectionSlider m_SelectionSlider;     

        //camera variables
        private Transform m_Camera;                    
        private SelectionRadial m_SelectionRadial;     
        private Reticle m_Reticle;                     

        [SerializeField] private UIController m_UIController;           // Used to encapsulate the UI.
        [SerializeField] private InputWarnings m_InputWarnings;         // Tap warnings need to be on for the intro and outro but off for the game itself.

        [SerializeField]
        private CameraFade m_CameraFade;           // Reference to the script that fades the scene to black.

        //control movement of player
        [SerializeField] private PathWalker m_PathWalker;         

                             
        public bool IsPlaying { get; private set; }



        private IEnumerator Start()
        {
            m_CameraFade = Camera.main.GetComponent<CameraFade>();

            SessionData.SetGameType(m_GameType);

            m_Camera = Camera.main.transform;
            m_SelectionRadial = m_Camera.GetComponent<SelectionRadial>();
            m_Reticle = m_Camera.GetComponent<Reticle>();
            m_PathWalker = m_Camera.GetComponent<PathWalker>();

            m_Camera.GetComponent<CameraController>().enabled = true;
            m_Camera.GetComponent<HideLockMouse>().enabled = true;

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

            m_Reticle.Show ();
            m_SelectionRadial.Hide ();

            m_InputWarnings.TurnOnDoubleTapWarnings ();
            m_InputWarnings.TurnOnSingleTapWarnings ();
            yield return StartCoroutine (m_SelectionSlider.WaitForBarToFill ());

            m_InputWarnings.TurnOffDoubleTapWarnings ();
            m_InputWarnings.TurnOffSingleTapWarnings ();
            yield return StartCoroutine (m_UIController.HideIntroUI ());
        }


        private IEnumerator PlayPhase ()
        {
            // Wait for the UI on the player's gun to fade in.
            yield return StartCoroutine(m_UIController.ShowPlayerUI());

            IsPlaying = true;
            m_Reticle.Show ();

            // Reset the score.
            SessionData.Restart ();

            for(int i = 0; i < m_PathWalker.stopPoints.Count; i++)
            {
                yield return StartCoroutine(m_PathWalker.PlayUpdate());
                //yield return StartCoroutine(PlayUpdate());
            }

            //gun UI fade out
            yield return StartCoroutine(m_UIController.HidePlayerUI());
            IsPlaying = false;
        }


        private IEnumerator EndPhase ()
        {
            m_Reticle.Hide ();
            yield return StartCoroutine (m_UIController.ShowOutroUI ());
            
            m_InputWarnings.TurnOnDoubleTapWarnings();
            m_InputWarnings.TurnOnSingleTapWarnings();
            yield return StartCoroutine(m_SelectionRadial.WaitForSelectionRadialToFill());
           
            m_InputWarnings.TurnOffDoubleTapWarnings();
            m_InputWarnings.TurnOffSingleTapWarnings();
            yield return StartCoroutine(m_UIController.HideOutroUI());

            m_PathWalker.Reset();

            //reset transform of camera
            yield return StartCoroutine(m_CameraFade.BeginFadeOut(true));
            Camera.main.transform.position = m_Start.position;
            Camera.main.transform.rotation = m_Start.rotation;
            m_Camera = Camera.main.transform;
            yield return StartCoroutine(m_CameraFade.BeginFadeIn(true));
        }
    }
}