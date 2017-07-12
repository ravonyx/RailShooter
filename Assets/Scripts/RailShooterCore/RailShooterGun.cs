using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    // This script controls the gun for the shooter
    // scenes, including it's movement and shooting.
    public class RailShooterGun : MonoBehaviour
    {
        //VR Parameters

        // The damping with which this gameobject follows the camera.
        [SerializeField]
        private float m_Damping = 0.5f;                                
        // How fast the gun arm follows the reticle.
        [SerializeField]
        private float m_GunContainerSmoothing = 10f;
        // This is the coefficient used to ensure smooth damping of this gameobject.
        private const float k_DampingCoef = -20f;

        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;

        [SerializeField]
        private AudioSource m_GunAudio;                                
        //ref to controller => to know if game is playing
        [SerializeField]
        private RailShooterController m_ShootingGalleryController; 


        [SerializeField]
        private float m_Speed;

        [SerializeField]
        private PKFxFX m_ShootParticles;
        [SerializeField]
        private PKFxFX m_impactParticles;
        [SerializeField]
        private float m_DefaultLineLength = 70f;
        [SerializeField]
        private float m_GunFlareVisibleSeconds = 0.1f;
        [SerializeField]
        private RailShooterPlayer m_playerHealth;

		//Line renderer native does not work with VR
		private LineRenderer m_gunFlare;   
		private VRLineRenderer m_vrGunFlare;   

        private Inputs m_inputs;
        private Raycaster m_raycaster;
        private Transform m_cameraTransform;
        private Reticle m_VRReticle;

        private Transform m_gunContainer;
        private Transform m_gunEnd;

        private bool m_init;

        private void OnEnable ()
        {
            m_inputs = m_camInputManager.CurrentInputs;
            m_inputs.OnDown += HandleDown;
            m_inputs.OnDownLeft += HandleDownLeft;
        }

        private void OnDisable ()
        {
            m_inputs.OnDown -= HandleDown;
            m_inputs.OnDownLeft -= HandleDownLeft;
        }

        void Start()
        {
            m_gunEnd = null;
            m_gunContainer = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.tag == "GunContainer")
                {
                    m_gunContainer = child;
                    for (int j = 0; j < m_gunContainer.childCount; j++)
                    {
                        child = m_gunContainer.GetChild(j);
                        if (child.tag == "GunEnd")
                        {
                            m_gunEnd = child;
                            break;
                        }
                    }

                }
            }

            if (m_camInputManager.CurrentInputName == "Mouse" || m_camInputManager.CurrentInputName == "Gamepad")
            {
                Camera cam = m_camInputManager.CurrentCamera;
                m_raycaster = cam.GetComponent<Raycaster>();
                m_cameraTransform = cam.transform;
                if (m_camInputManager.CurrentInputName == "Gamepad")
                    m_VRReticle = cam.GetComponent<Reticle>();
            }

            else if (m_camInputManager.CurrentInputName == "Touch")
                m_raycaster = GetComponent<Raycaster>();

            if (VRSettings.enabled)
				m_vrGunFlare = GetComponent<VRLineRenderer> ();
			else
				m_gunFlare = GetComponent<LineRenderer> ();
        }

        private void Update()
        {
            //move the weapon in function of Type of Inputs
            if (m_camInputManager.CurrentInputName == "Mouse")
            {
                transform.position = m_cameraTransform.position;
                transform.rotation = m_cameraTransform.rotation;
            }
            else if (m_camInputManager.CurrentInputName == "Gamepad")
            {
                transform.position = m_cameraTransform.position;

                if (m_camInputManager.CurrentCamera.name == "VRCamera")
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, InputTracking.GetLocalRotation(VRNode.Head),
                    m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

                    Quaternion lookAtRotation = Quaternion.LookRotation(m_VRReticle.ReticleTransform.position - m_gunContainer.position);
                    m_gunContainer.rotation = Quaternion.Slerp(m_gunContainer.rotation, lookAtRotation, m_GunContainerSmoothing * Time.deltaTime);
                }
                else if (m_camInputManager.CurrentCamera.name == "FoveInterface")
                {
                    Quaternion quat = FoveInterface.GetHMDRotation();
                    transform.rotation = Quaternion.Slerp(transform.rotation, quat,
                    m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

                    Quaternion lookAtRotation = Quaternion.LookRotation(m_VRReticle.ReticleTransform.position - m_gunContainer.position);
                    m_gunContainer.rotation = Quaternion.Slerp(m_gunContainer.rotation, lookAtRotation, m_GunContainerSmoothing * Time.deltaTime);
                }
            }

            else if (m_camInputManager.CurrentInputName == "Touch")
            {
                //Update anchors of touch weapon
                OVRInput.Update();
                if (tag == "LTouch")
                {
                    transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
                    transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
                }
                else if (tag == "RTouch")
                {
                    transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                    transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                }
            }
        }

        private void HandleDown ()
        {
           if (!m_ShootingGalleryController.IsPlaying || tag == "LTouch")
                return;

            RailShooterEntity shootingTarget = m_raycaster.CurrentInteractible ? m_raycaster.CurrentInteractible.GetComponent<RailShooterEntity>() : null;
            Transform target = shootingTarget ? shootingTarget.transform : null;
            StartCoroutine(Fire(target));
        }

        private void HandleDownLeft()
        {
            if (!m_ShootingGalleryController.IsPlaying || tag == "RTouch")
                return;

            RailShooterEntity shootingTarget = m_raycaster.CurrentInteractible ? m_raycaster.CurrentInteractible.GetComponent<RailShooterEntity>() : null;
            Transform target = shootingTarget ? shootingTarget.transform : null;
            StartCoroutine(Fire(target));

        }
        private IEnumerator Fire(Transform target)
        {
            if (target != null && target.name == "Health")
                m_playerHealth.StartCoroutine("EvolveLife");


            m_GunAudio.Play();
            float lineLength = m_DefaultLineLength;
            if (target)
                lineLength = Vector3.Distance(m_gunEnd.position, target.position);

            yield return new WaitForEndOfFrame();

            m_impactParticles.StartEffect();
            //shoot line + muzzle flash
            m_ShootParticles.StartEffect();

			if(VRSettings.enabled)
				m_vrGunFlare.enabled = true;
			else
				m_gunFlare.enabled = true;

            yield return StartCoroutine(MoveLineRenderer(lineLength));

			if(VRSettings.enabled)
				m_vrGunFlare.enabled = false;
			else
				m_gunFlare.enabled = false;
        }

        private IEnumerator MoveLineRenderer(float lineLength)
        {
            float timer = 0f;
            while (timer < m_GunFlareVisibleSeconds)
            {

				if (VRSettings.enabled) 
				{
					m_vrGunFlare.SetPosition (0, m_gunEnd.position);
					m_vrGunFlare.SetPosition (1, m_gunEnd.position + m_gunEnd.forward * 5000.0f);
				}
                if (!VRSettings.enabled)
                {
					m_gunFlare.SetPosition(0, m_gunEnd.position);
					m_gunFlare.SetPosition(1, m_gunEnd.position + m_gunEnd.forward * lineLength);

				}
             
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }
}