using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;
using UnityEngine.VR;

namespace Assets.RailShooter
{
    // This class controls the flow of the rail shooter games.  It
    // includes the introduction, spawning of targets and
    // outro.  Targets are spawned with a system that makes
    // spawnning more likely when there are fewer.

    public class RailShooterController : MonoBehaviour
    {
        [SerializeField] private SessionData.GameType m_GameType;      

        [SerializeField] private int m_IdealTargetNumber = 5;           // How many targets aim to be on screen at once.
        [SerializeField] private float m_BaseSpawnProbability = 0.7f;   // When there are the ideal number of targets, this is the probability another will spawn.
        [SerializeField] private float m_GameLength = 60f;              // Time a game lasts in seconds.
        [SerializeField] private float m_SpawnInterval = 1f;            // How frequently a target could spawn.
        [SerializeField] private float m_EndDelay = 1.5f;               // The time the user needs to wait between the outro UI and being able to continue.

        //shooter 360 variables
        [SerializeField] private float m_SphereSpawnInnerRadius = 5f;   
        [SerializeField] private float m_SphereSpawnOuterRadius = 10f; 
        [SerializeField] private float m_SphereSpawnMaxHeight = 15f;    

        [SerializeField] private SelectionSlider m_SelectionSlider;     

        //camera variables
        private Transform m_Camera;                    
        private SelectionRadial m_SelectionRadial;     
        private Reticle m_Reticle;                     

        [SerializeField] private Image m_TimerBar;                      // The time remaining is shown on the UI for the gun, this is a reference to the image showing the time remaining.
        [SerializeField] private ObjectPool m_TargetObjectPool;         // The object pool that stores all the targets.
        [SerializeField] private UIController m_UIController;           // Used to encapsulate the UI.
        [SerializeField] private InputWarnings m_InputWarnings;         // Tap warnings need to be on for the intro and outro but off for the game itself.

        //control movement of player
        [SerializeField] private SplineWalker m_SplineWalker;         

        private float m_SpawnProbability;                               
        private float m_ProbabilityDelta;          
                             
        public bool IsPlaying { get; private set; }                     

        private IEnumerator Start()
        {
            SessionData.SetGameType(m_GameType);
            m_ProbabilityDelta = (1f - m_BaseSpawnProbability) / m_IdealTargetNumber;

            m_Camera = Camera.main.transform;
            m_SelectionRadial = m_Camera.GetComponent<SelectionRadial>();
            m_Reticle = m_Camera.GetComponent<Reticle>();
			m_SplineWalker = m_Camera.GetComponent<SplineWalker>();

            if (SessionData.GetGameType() == SessionData.GameType.SHOOTER360)
            {
                m_Camera.GetComponent<CameraController>().enabled = true;
                m_Camera.GetComponent<HideLockMouse>().enabled = true;
            }
            else
                m_Camera.GetComponent<CameraController>().enabled = false;

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
            // Wait for the intro UI to fade in.
            yield return StartCoroutine (m_UIController.ShowIntroUI ());

            // Show the reticle (since there is now a selection slider) and hide the radial.
            m_Reticle.Show ();
            m_SelectionRadial.Hide ();

            // Turn on the tap warnings for the selection slider.
            m_InputWarnings.TurnOnDoubleTapWarnings ();
            m_InputWarnings.TurnOnSingleTapWarnings ();

            // Wait for the selection slider to finish filling.
            yield return StartCoroutine (m_SelectionSlider.WaitForBarToFill ());

            // Turn off the tap warnings since it will now be tap to fire.
            m_InputWarnings.TurnOffDoubleTapWarnings ();
            m_InputWarnings.TurnOffSingleTapWarnings ();

            // Wait for the intro UI to fade out.
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

            for(int i = 0; i < m_SplineWalker.stopPoints.Count; i++)
            {
                //walk at next point
                yield return StartCoroutine(m_SplineWalker.StartPhase());
                yield return StartCoroutine(m_SplineWalker.PlayUpdate());
                //spawn update 
                yield return StartCoroutine(PlayUpdate());
            }

            //gun UI fade out
            yield return StartCoroutine(m_UIController.HidePlayerUI());
            IsPlaying = false;
        }


        private IEnumerator EndPhase ()
        {
            // Hide the reticle since the radial is about to be used.
            m_Reticle.Hide ();
            
            // In order, wait for the outro UI to fade in then wait for an additional delay.
            yield return StartCoroutine (m_UIController.ShowOutroUI ());
            yield return new WaitForSeconds(m_EndDelay);
            
            // Turn on the tap warnings.
            m_InputWarnings.TurnOnDoubleTapWarnings();
            m_InputWarnings.TurnOnSingleTapWarnings();

            // Wait for the radial to fill (this will show and hide the radial automatically).
            yield return StartCoroutine(m_SelectionRadial.WaitForSelectionRadialToFill());

            // The radial is now filled so stop the warnings.
            m_InputWarnings.TurnOffDoubleTapWarnings();
            m_InputWarnings.TurnOffSingleTapWarnings();

            // Wait for the outro UI to fade out.
            yield return StartCoroutine(m_UIController.HideOutroUI());
        }


        private IEnumerator PlayUpdate ()
        {
            m_SpawnProbability = 1f;
            float gameTimer = m_GameLength;
            float spawnTimer = m_SpawnInterval;

            while (gameTimer > 0f)
            {
                if (spawnTimer <= 0f)
                {
                    if (Random.value < m_SpawnProbability)
                    {
                        spawnTimer = m_SpawnInterval;
                        m_SpawnProbability -= m_ProbabilityDelta;
                        Spawn (gameTimer);
                    }
                }
                yield return null;

                gameTimer -= Time.deltaTime;
                spawnTimer -= Time.deltaTime;

                m_TimerBar.fillAmount = gameTimer / m_GameLength;
            }
        }


        private void Spawn (float timeRemaining)
        {
            GameObject target = m_TargetObjectPool.GetGameObjectFromPool ();
            //random position. 
            target.transform.position = SpawnPosition();

            //shooting target restart
            RailShooterTarget shootingTarget = target.GetComponent<RailShooterTarget>();
            shootingTarget.Restart(timeRemaining);

            //on remove event
            shootingTarget.OnRemove += HandleTargetRemoved;
        }


        private Vector3 SpawnPosition ()
        {
            //this game is a 180 
            if (m_GameType == SessionData.GameType.SERIOUSSHOOTER)
            {
                float x = Random.value;
                float y = Random.value;
                if (x == 0)
                    x = 0.1f;
                else if (x == 1)
                    x = 0.9f;
                if (y == 0)
                    y = 0.1f;
                else if (y == 1)
                    y = 0.9f;

                var pos = new Vector3(Random.value, Random.value, 5);
                pos = Camera.main.ViewportToWorldPoint(pos);

                return new Vector3(pos.x, pos.y, pos.z);
            }

            //Otherwise the game is 360
            else
            {
                Vector2 randomCirclePoint = Random.insideUnitCircle * Random.Range(m_SphereSpawnInnerRadius, m_SphereSpawnOuterRadius) +
                    new Vector2(m_Camera.transform.position.x, m_Camera.transform.position.z);
                float randomHeight = Random.Range(m_Camera.position.y, m_SphereSpawnMaxHeight);
                return new Vector3(randomCirclePoint.x, randomHeight, randomCirclePoint.y);
            }
        }

        private void HandleTargetRemoved(RailShooterTarget target)
        {
            target.OnRemove -= HandleTargetRemoved;
            m_TargetObjectPool.ReturnGameObjectToPool (target.gameObject);
            m_SpawnProbability += m_ProbabilityDelta;
        }
    }
}