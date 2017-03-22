using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;
using UnityEngine.VR;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    // This class controls the flow of the rail shooter games.  It
    // includes the introduction, spawning of targets and
    // outro.  Targets are spawned with a system that makes
    // spawnning more likely when there are fewer.

    public class RailShooterController : MonoBehaviour
    {
        [SerializeField] private SessionData.GameType m_GameType;      

        //values for spawning enemies
        [SerializeField] private int m_IdealTargetNumber = 5;           
        [SerializeField] private float m_BaseSpawnProbability = 0.7f;   
        [SerializeField] private float m_GameLength = 60f;              
        [SerializeField] private float m_SpawnInterval = 1f;
        //shooter 360 variables
        [SerializeField]
        private float m_SphereSpawnInnerRadius = 5f;
        [SerializeField]
        private float m_SphereSpawnOuterRadius = 10f;
        [SerializeField]
        private float m_SphereSpawnMaxHeight = 15f;

        [SerializeField] private Transform m_Start;              
        [SerializeField] private SelectionSlider m_SelectionSlider;     

        //camera variables
        private Transform m_Camera;                    
        private SelectionRadial m_SelectionRadial;     
        private Reticle m_Reticle;                     

        [SerializeField] private Image m_TimerBar;                      // The time remaining is shown on the UI for the gun, this is a reference to the image showing the time remaining.
        [SerializeField] private ObjectPool m_TargetObjectPool;         // The object pool that stores all the targets.
        [SerializeField] private UIController m_UIController;           // Used to encapsulate the UI.
        [SerializeField] private InputWarnings m_InputWarnings;         // Tap warnings need to be on for the intro and outro but off for the game itself.

        [SerializeField]
        private CameraFade m_CameraFade;           // Reference to the script that fades the scene to black.

        //control movement of player
        [SerializeField] private PathWalker m_PathWalker;         

        private float m_SpawnProbability;                               
        private float m_ProbabilityDelta;          
                             
        public bool IsPlaying { get; private set; }                     

        private IEnumerator Start()
        {
            m_CameraFade = Camera.main.GetComponent<CameraFade>();

            SessionData.SetGameType(m_GameType);
            m_ProbabilityDelta = (1f - m_BaseSpawnProbability) / m_IdealTargetNumber;

            m_Camera = Camera.main.transform;
            m_SelectionRadial = m_Camera.GetComponent<SelectionRadial>();
            m_Reticle = m_Camera.GetComponent<Reticle>();
            m_PathWalker = m_Camera.GetComponent<PathWalker>();

            if (SessionData.GetGameType() == SessionData.GameType.SHOOTER360)
            {
                m_Camera.GetComponent<CameraController>().enabled = true;
                m_Camera.GetComponent<HideLockMouse>().enabled = true;
            }
            else
            {
                m_Camera.GetComponent<CameraController>().enabled = false;
                m_Camera.GetComponent<HideLockMouse>().enabled = false;
            }

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
            if (SessionData.GetGameType() == SessionData.GameType.SHOOTER360)
                yield return StartCoroutine(m_UIController.ShowPlayerUI());

            IsPlaying = true;
            m_Reticle.Show ();

            // Reset the score.
            SessionData.Restart ();

            for(int i = 0; i < m_PathWalker.stopPoints.Count; i++)
            {
                //walk at next point
                yield return StartCoroutine(m_PathWalker.StartPhase());
                yield return StartCoroutine(m_PathWalker.PlayUpdate());
                //spawn update 
                yield return StartCoroutine(PlayUpdate());
            }

            //gun UI fade out
            if (SessionData.GetGameType() == SessionData.GameType.SHOOTER360)
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
                float offset = 0.5f;
                if (x == 0)
                    x += offset;
                else if (x == 1)
                    x -= offset;
                if (y == 0)
                    y += offset;
                else if (y == 1)
                    y -= offset;

                var pos = new Vector3(x, y, 5);
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