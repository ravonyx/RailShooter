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

        [SerializeField] private AudioSource m_GunAudio;                                
        //ref to controller => to know if game is playing
        [SerializeField] private RailShooterController m_ShootingGalleryController; 

        [SerializeField] private VREyeRaycaster m_EyeRaycaster;                         
        [SerializeField] private MouseRaycaster m_MouseRayCaster;
        [SerializeField] private VRInput m_VRInput;                                     
        [SerializeField] private MouseInput m_MouseInput;

        [SerializeField] private Transform m_VRCameraTransform;                           
        [SerializeField] private Transform m_MouseCameraTransform;                          

        [SerializeField] private Transform m_GunContainer;                             
        [SerializeField] private Transform m_GunEnd;         
        
        //reticle to know where firing                         
        [SerializeField] private Reticle m_VRReticle;                                    
        [SerializeField] private Reticle m_MouseReticle;                                   

        [SerializeField] private ObjectPool m_ProjectilesPool;
        [SerializeField] private float m_Speed;

        [SerializeField]
        private PKFxFX m_ShootParticles;

        [SerializeField]
        private float m_DefaultLineLength = 70f;
        [SerializeField]
        private LineRenderer m_GunFlare;   
        [SerializeField]
        private ParticleSystem m_FlareParticles;                     
        [SerializeField]
        private float m_GunFlareVisibleSeconds = 0.07f;                

        private void OnEnable ()
        {
            if (VRSettings.enabled == false)
                m_MouseInput.OnDown += HandleDown;
            else
                m_VRInput.OnDown += HandleDown;
        }

        private void OnDisable ()
        {
            if (VRSettings.enabled == false)
                m_MouseInput.OnDown -= HandleDown;
            else
                m_VRInput.OnDown -= HandleDown;
        }

        private void Update()
        {
            if (!VRSettings.enabled)
            {
                transform.rotation = m_MouseCameraTransform.rotation;
                transform.position = m_MouseCameraTransform.position;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, InputTracking.GetLocalRotation(VRNode.Head),
              m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

                transform.position = m_VRCameraTransform.position;
                Debug.Log(transform.position);

                Quaternion lookAtRotation = Quaternion.LookRotation(m_VRReticle.ReticleTransform.position - m_GunContainer.position);
                m_GunContainer.rotation = Quaternion.Slerp(m_GunContainer.rotation, lookAtRotation,
                    m_GunContainerSmoothing * Time.deltaTime);
            }
        }

        private void HandleDown ()
        {
           if (!m_ShootingGalleryController.IsPlaying)
                return;

            RailShooterTarget shootingTarget;
            if (VRSettings.enabled)
                shootingTarget = m_EyeRaycaster.CurrentInteractible ? m_EyeRaycaster.CurrentInteractible.GetComponent<RailShooterTarget>() : null;
            else
                shootingTarget = m_MouseRayCaster.CurrentInteractible ? m_MouseRayCaster.CurrentInteractible.GetComponent<RailShooterTarget>() : null;

            Transform target = shootingTarget ? shootingTarget.transform : null;
            StartCoroutine(Fire(target));
        }

        private IEnumerator Fire(Transform target)
        {
            m_GunAudio.Play();
            float lineLength = m_DefaultLineLength;

           if (target)
               lineLength = Vector3.Distance(m_GunEnd.position, target.position);

            yield return new WaitForEndOfFrame();

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