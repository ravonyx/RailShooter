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
        [SerializeField] private float m_Damping = 0.5f;                                // The damping with which this gameobject follows the camera.
        [SerializeField] private float m_GunContainerSmoothing = 10f;                   // How fast the gun arm follows the reticle.
        [SerializeField] private AudioSource m_GunAudio;                                // The audio source which plays the sound of the gun firing.
        [SerializeField] private RailShooterController m_ShootingGalleryController; // Reference to the controller so the gun cannot fire whilst the game isn't playing.
        [SerializeField] private VREyeRaycaster m_EyeRaycaster;                         // Used to detect whether the gun is currently aimed at something.
        [SerializeField] private VRInput m_VRInput;                                     // Used to tell the gun when to fire.
        [SerializeField] private MouseRaycaster m_MouseRaycaster;
        [SerializeField] private MouseInput m_MouseInput;

        [SerializeField] private Transform m_VRCameraTransform;                           // Used as a reference to move this gameobject towards.
        [SerializeField] private Transform m_MouseCameraTransform;                           // Used as a reference to move this gameobject towards.

        [SerializeField] private Transform m_GunContainer;                              // This contains the gun arm needs to be moved smoothly.
        [SerializeField] private Transform m_GunEnd;                                    // This is where the line renderer should start from.
        [SerializeField] private Reticle m_VRReticle;                                     // This is what the gun arm should be aiming at.
        [SerializeField] private Reticle m_MouseReticle;                                     // This is what the gun arm should be aiming at.

        [SerializeField] private ObjectPool m_ProjectilesPool;

        private const float k_DampingCoef = -20f;                                       // This is the coefficient used to ensure smooth damping of this gameobject.

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

            RailShooterTarget shootingTarget = m_EyeRaycaster.CurrentInteractible ? m_EyeRaycaster.CurrentInteractible.GetComponent<RailShooterTarget>() : null;
            Transform target = shootingTarget ? shootingTarget.transform : null;

            Fire(target);
        }


        private void Fire(Transform target)
        {
            m_GunAudio.Play();

            ShooterBullet m_Projectile = m_ProjectilesPool.GetGameObjectFromPool();

            m_Projectile.transform.position = m_GunEnd.transform.position;
            m_Projectile.Rb.AddForce(m_GunEnd.forward * 1500.0f);
        }

    }
}