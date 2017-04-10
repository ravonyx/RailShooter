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
				return;

			transform.rotation = Quaternion.Slerp(transform.rotation, InputTracking.GetLocalRotation(VRNode.Head),
				m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

			transform.position = m_VRCameraTransform.position;
			Debug.Log(transform.position);

			Quaternion lookAtRotation = Quaternion.LookRotation(m_VRReticle.ReticleTransform.position - m_GunContainer.position);
			m_GunContainer.rotation = Quaternion.Slerp(m_GunContainer.rotation, lookAtRotation,
				m_GunContainerSmoothing * Time.deltaTime);
		}

        void LateUpdate()
        {
			if (!VRSettings.enabled)
			{
				transform.rotation = m_MouseCameraTransform.transform.rotation;
				transform.position = m_MouseCameraTransform.position;
			}
		}

        private void HandleDown ()
        {
            if (!m_ShootingGalleryController.IsPlaying)
                return;

            Vector3 shootingTarget = Vector3.zero;
            if (VRSettings.enabled == true)
                shootingTarget = m_VRReticle.ReticleTransform.position;
            else
                shootingTarget = m_MouseReticle.ReticleTransform.position;

            Fire(shootingTarget);
        }


        private void Fire(Vector3 target)
        {
            m_GunAudio.Play();

            Ray r = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit rh = new RaycastHit();
            Vector3 aimPoint = r.GetPoint(500.0f);
            if (Physics.Raycast(r, out rh))
                aimPoint = rh.point;
            
            ShooterBullet projectile = m_ProjectilesPool.GetGameObjectFromPool();
            Vector3 direction = (aimPoint - m_GunEnd.position).normalized;

            Debug.DrawRay(m_GunEnd.position, direction, Color.red, 5.0f);

            projectile.transform.parent = m_GunEnd;
            projectile.transform.position = m_GunEnd.position;
            projectile.transform.parent = null;

            projectile.Rigidbody.AddForce(direction * m_Speed);
        }
    }
}