using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;

namespace Assets.RailShooter
{
    // This class controls the flow of the rail shooter games.  It
    // includes the introduction, spawning of targets and
    // outro.  Targets are spawned with a system that makes
    // spawnning more likely when there are fewer.

    public class RailShooterController : MonoBehaviour
    {
        [SerializeField] private SessionData.GameType m_GameType;       // Whether this is a 180 or 360 shooter.

        [SerializeField] private int m_IdealTargetNumber = 5;           // How many targets aim to be on screen at once.
        [SerializeField] private float m_BaseSpawnProbability = 0.7f;   // When there are the ideal number of targets, this is the probability another will spawn.
        [SerializeField] private float m_GameLength = 60f;              // Time a game lasts in seconds.
        [SerializeField] private float m_SpawnInterval = 1f;            // How frequently a target could spawn.
        [SerializeField] private float m_EndDelay = 1.5f;               // The time the user needs to wait between the outro UI and being able to continue.

        //shooter 360 variables
        [SerializeField] private float m_SphereSpawnInnerRadius = 5f;   
        [SerializeField] private float m_SphereSpawnOuterRadius = 10f; 
        [SerializeField] private float m_SphereSpawnMaxHeight = 15f;    

        [SerializeField] private SelectionSlider m_SelectionSlider;     // Used to confirm the user has understood the intro UI.

        //camera variables
        [SerializeField] private Transform m_Camera;                    // Used to determine where targets can spawn.
        [SerializeField] private SelectionRadial m_SelectionRadial;     // Used to continue past the outro.
        [SerializeField] private Reticle m_Reticle;                     // This is turned on and off when it is required and not.

        [SerializeField] private Image m_TimerBar;                      // The time remaining is shown on the UI for the gun, this is a reference to the image showing the time remaining.
        [SerializeField] private ObjectPool m_TargetObjectPool;         // The object pool that stores all the targets.
        [SerializeField] private BoxCollider m_SpawnCollider;           // For the 180 shooter, the volume that the targets can spawn within.
        [SerializeField] private UIController m_UIController;           // Used to encapsulate the UI.
        [SerializeField] private InputWarnings m_InputWarnings;         // Tap warnings need to be on for the intro and outro but off for the game itself.


        //control movement of player
        [SerializeField] private SplineWalker m_SplineWalker;         

        private float m_SpawnProbability;                               // The current probability that a target will spawn at the next interval.
        private float m_ProbabilityDelta;                               // The difference to the probability caused by a target spawning or despawning.

        public bool IsPlaying { get; private set; }                     // Whether or not the game is currently playing.


        private IEnumerator Start()
        {
            // Set the game type for the score to be recorded correctly.
            SessionData.SetGameType(m_GameType);

            // The probability difference for each spawn is difference between 100% and the base probabilty per the number or targets wanted.
            // That means that if the ideal number of targets was 5, the base probability was 0.7 then the delta is 0.06.
            // So if there are no targets, the probability of one spawning will be 1, then 0.94, then 0.88, etc.
            m_ProbabilityDelta = (1f - m_BaseSpawnProbability) / m_IdealTargetNumber;

            m_Camera = Camera.main.transform;
            m_SelectionRadial = m_Camera.GetComponent<SelectionRadial>();
            m_Reticle = m_Camera.GetComponent<Reticle>();

            // Continue looping through all the phases.
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
                // Find the centre and extents of the spawn collider.
                Vector3 center = m_SpawnCollider.bounds.center;
                Vector3 extents = m_SpawnCollider.bounds.extents;

                // Get a random value between the extents on each axis.
                float x = Random.Range(center.x - extents.x, center.x + extents.x);
                float y = Random.Range(center.y - extents.y, center.y + extents.y);
                float z = Random.Range(center.z - extents.z, center.z + extents.z);
                
                // Return the point these random values make.
                return new Vector3(x, y, z);
            }

            //Otherwise the game is 360 a
            //random point on a unit circle and give it a radius between the inner and outer radii.
            Vector2 randomCirclePoint = Random.insideUnitCircle * Random.Range(m_SphereSpawnInnerRadius, m_SphereSpawnOuterRadius) +
                new Vector2(m_Camera.transform.position.x, m_Camera.transform.position.z);

            // Find a random height between the camera's height and the maximum.
            float randomHeight = Random.Range (m_Camera.position.y, m_SphereSpawnMaxHeight);

            // The the random point on the circle is on the XZ plane and the random height is the Y axis.
            return new Vector3(randomCirclePoint.x, randomHeight, randomCirclePoint.y);
        }


        private void HandleTargetRemoved(RailShooterTarget target)
        {
            // Now that the event has been hit, unsubscribe from it.
            target.OnRemove -= HandleTargetRemoved;

            // Return the target to it's object pool.
            m_TargetObjectPool.ReturnGameObjectToPool (target.gameObject);

            // Increase the likelihood of a spawn next time because there are fewer targets now.
            m_SpawnProbability += m_ProbabilityDelta;
        }
    }
}