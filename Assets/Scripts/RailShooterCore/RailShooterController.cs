using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using UnityEngine.VR;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    public class RailShooterController : MonoBehaviour
    {
        [SerializeField] private SessionData.GameType m_gameType;      

        [SerializeField] 
		private Transform m_start;              
        [SerializeField] 
		private SelectionSlider m_selectionSlider;
        [SerializeField]
		private CamerasAndInputsManager m_camInputsManager;
		[SerializeField]
		private GameObject m_touch;

        private Transform m_camera;      
        private Reticle m_reticle;
        private SelectionRadial m_selectionRadial;    

        [SerializeField]
        private bool m_tutorial;
        [SerializeField]
        private UIController m_UIController;           // Used to encapsulate the UI.
        [SerializeField]
        private InputWarnings m_inputWarnings;         // Tap warnings need to be on for the intro and outro but off for the game itself.

        [SerializeField]
        private CameraFade m_cameraFade;           // Reference to the script that fades the scene to black.

        [SerializeField]
        private AudioClip m_gameOverClip;
        [SerializeField]
        private AudioClip m_levelDoneClip;
        [SerializeField]
        private AudioSource m_audioSourceMusic;
        [SerializeField]
        private AudioSource m_audioSourceSFX;

        [SerializeField]
        PKFxFX []m_endFX;

        //control movement of player
        private PathWalker m_pathWalker;         
        public bool IsPlaying { get; private set; }
        private int m_stepTutorial;

        private IEnumerator Start()
        {
			Camera camera = m_camInputsManager.CurrentCamera;
			m_cameraFade = camera.GetComponent<CameraFade>();
			m_camera = camera.transform;
			
			m_pathWalker = camera.GetComponentInParent<PathWalker>();


            if (m_camInputsManager.CurrentInputName == "Touch")
            {

                m_selectionRadial = m_touch.GetComponent<SelectionRadial>();
                m_reticle = m_touch.GetComponent<Reticle>();
            }
            else
            {
                m_selectionRadial = camera.GetComponent<SelectionRadial>();
                m_reticle = camera.GetComponent<Reticle>();
            }
            SessionData.SetGameType(m_gameType);
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
            yield return StartCoroutine (m_selectionSlider.WaitForBarToFill ());
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
                    m_stepTutorial = -1;
                    yield return StartCoroutine(m_pathWalker.PlayUpdate());
                    if (i == 0)
                    {
                        m_stepTutorial = 0;
                        IsPlaying = false;
                        yield return StartCoroutine(m_UIController.ShowEnemiesUI());
                        yield return StartCoroutine(m_selectionRadial.WaitForSelectionRadialToFill());
                        yield return StartCoroutine(m_UIController.HideEnemiesUI());
                        IsPlaying = true;
                    }
                    else if(i == 1)
                    {
                        m_stepTutorial = 1;
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
        }


        private IEnumerator EndPhase ()
        {
            m_audioSourceMusic.Stop();

            m_audioSourceSFX.clip = m_levelDoneClip;
            m_audioSourceSFX.PlayOneShot(m_levelDoneClip, 1.0f);


            for (int i = 0; i < m_endFX.Length; i++)
                m_endFX[i].StartEffect();

            //hide the explanations UI if game over before end
            m_reticle.Hide ();
            yield return StartCoroutine (m_UIController.ShowOutroUI ());
            
            m_inputWarnings.TurnOnDoubleTapWarnings();
            m_inputWarnings.TurnOnSingleTapWarnings();
            yield return StartCoroutine(m_selectionRadial.WaitForSelectionRadialToFill());

            m_inputWarnings.TurnOffDoubleTapWarnings();
            m_inputWarnings.TurnOffSingleTapWarnings();
            yield return StartCoroutine(m_UIController.HideOutroUI());

            yield return StartCoroutine(m_cameraFade.BeginFadeOut(true));
            m_pathWalker.Reset();

            m_camera.transform.parent.localPosition = m_start.position;
            m_camera.transform.parent.localPosition = m_start.position;

            m_camera.transform.localRotation = m_start.rotation;
            yield return StartCoroutine(m_cameraFade.BeginFadeIn(true));

            StartCoroutine(Start());
        }

        public IEnumerator GameOverPhase()
        {
            m_audioSourceMusic.Stop();
            m_audioSourceSFX.clip = m_gameOverClip;
            m_audioSourceSFX.PlayOneShot(m_gameOverClip, 1.0f);

            IsPlaying = false;
            StopAllCoroutines();
            m_pathWalker.Walking = false;

            if (m_tutorial)
            {
                if (m_stepTutorial == 0)
                    yield return StartCoroutine(m_UIController.HideEnemiesUI());
                if (m_stepTutorial == 1)
                    yield return StartCoroutine(m_UIController.HideHealthUI());
            }

            m_selectionRadial.Hide();

            //hide the explanations UI if game over before end
            m_reticle.Hide();
            yield return StartCoroutine(m_UIController.ShowGameOverUI());

            m_inputWarnings.TurnOnDoubleTapWarnings();
            m_inputWarnings.TurnOnSingleTapWarnings();
            yield return StartCoroutine(m_selectionRadial.WaitForSelectionRadialToFill());

            m_inputWarnings.TurnOffDoubleTapWarnings();
            m_inputWarnings.TurnOffSingleTapWarnings();
            yield return StartCoroutine(m_UIController.HideGameOverUI());

            yield return StartCoroutine(m_cameraFade.BeginFadeOut(true));
            m_pathWalker.Reset();

            m_camera.transform.parent.localPosition = m_start.position;
            m_camera.transform.parent.localPosition = m_start.position;

            m_camera.transform.localRotation = m_start.rotation;
            yield return StartCoroutine(m_cameraFade.BeginFadeIn(true));

            StartCoroutine(Start());
        }
    }
}