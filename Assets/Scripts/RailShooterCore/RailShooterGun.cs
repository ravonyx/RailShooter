using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using VRStandardAssets.Utils;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    // This script controls the gun for the shooter
    // scenes, including it's movement and shooting.
    public class RailShooterGun : MonoBehaviour
    {
        //VR Parameters

        // The damping with which this gameobject follows the camera.
        [SerializeField] private float m_Damping = 0.5f;                                
        // How fast the gun arm follows the reticle.
        [SerializeField] private float m_GunContainerSmoothing = 10f;
        // This is the coefficient used to ensure smooth damping of this gameobject.
        private const float k_DampingCoef = -20f;

        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;

        [SerializeField] private AudioSource m_GunAudio;                                
        //ref to controller => to know if game is playing
        [SerializeField] private RailShooterController m_ShootingGalleryController; 

        [SerializeField] private VREyeRaycaster m_EyeRaycaster;                         
        [SerializeField] private MouseRaycaster m_MouseRayCaster;

        [SerializeField] private Transform m_GunContainer;                             
        [SerializeField] private Transform m_GunEnd;         
        
        [SerializeField] private float m_Speed;

        [SerializeField]
        private PKFxFX m_ShootParticles;

        [SerializeField]
        private float m_DefaultLineLength = 70f;
        [SerializeField]
        private LineRenderer m_GunFlare;   
        [SerializeField]
        private float m_GunFlareVisibleSeconds = 0.1f;

        [SerializeField]
        private RailShooterPlayer m_playerHealth;

        private Inputs m_inputs;
        private Transform m_cameraTransform;
        //reticle to know where firing                         
        private Reticle m_VRReticle;

        private void OnEnable ()
        {
            m_inputs = m_camInputManager.GetCurrentInputs();
            m_inputs.OnDown += HandleDown;
        }

        private void OnDisable ()
        {
            m_inputs.OnDown -= HandleDown;
        }

        void Start()
        {
            Camera cam = m_camInputManager.CurrentCamera;
            m_cameraTransform = cam.transform;
            m_VRReticle = cam.GetComponent<Reticle>();
        }

        private void Update()
        {
            if (!VRSettings.enabled)
            {
                transform.rotation = m_cameraTransform.rotation;
                transform.position = m_cameraTransform.position;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, InputTracking.GetLocalRotation(VRNode.Head),
              m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

                transform.position = m_cameraTransform.position;

                Quaternion lookAtRotation = Quaternion.LookRotation(m_VRReticle.ReticleTransform.position - m_GunContainer.position);
                m_GunContainer.rotation = Quaternion.Slerp(m_GunContainer.rotation, lookAtRotation,
                    m_GunContainerSmoothing * Time.deltaTime);
            }
        }

        private void HandleDown ()
        {
           if (!m_ShootingGalleryController.IsPlaying)
                return;

            RailShooterEntity shootingTarget;
            if (VRSettings.enabled)
                shootingTarget = m_EyeRaycaster.CurrentInteractible ? m_EyeRaycaster.CurrentInteractible.GetComponent<RailShooterEntity>() : null;
            else
                shootingTarget = m_MouseRayCaster.CurrentInteractible ? m_MouseRayCaster.CurrentInteractible.GetComponent<RailShooterEntity>() : null;

            Transform target = shootingTarget ? shootingTarget.transform : null;
            StartCoroutine(Fire(target));
        }

        private IEnumerator Fire(Transform target)
        {
            if (target != null && target.name == "Health")
                m_playerHealth.StartCoroutine("EvolveLife", true);

            m_GunAudio.Play();
            float lineLength = m_DefaultLineLength;
            if (target)
                lineLength = Vector3.Distance(m_GunEnd.position, target.position);

            yield return new WaitForEndOfFrame();

            //shoot line + muzzle flash
            m_ShootParticles.StartEffect();

            m_GunFlare.enabled = true;
            yield return StartCoroutine(MoveLineRenderer(lineLength));
            m_GunFlare.enabled = false;
        }

        private IEnumerator MoveLineRenderer(float lineLength)
        {
            float timer = 0f;
            while (timer < m_GunFlareVisibleSeconds)
            {
                m_GunFlare.SetPosition(0, m_GunEnd.position);
                m_GunFlare.SetPosition(1, m_GunEnd.position + m_GunEnd.forward * lineLength);

                yield return null;
                timer += Time.deltaTime;
            }
        }
    }
}