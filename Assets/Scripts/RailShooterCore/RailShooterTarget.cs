using System;
using System.Collections;
using UnityEngine;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;
using RailShooter.Utils;

namespace Assets.RailShooter
{
    public class RailShooterTarget : MonoBehaviour
    {
        public event Action<RailShooterTarget> OnRemove;                   

        [SerializeField] private int m_Score = 1;                      

        [SerializeField] private float m_DestroyTimeOutDuration = 2f;   // When the target is hit, it shatters.  This is how long before the shattered pieces disappear.
        [SerializeField] private GameObject m_DestroyPrefab;            // The prefab for the shattered target.

        [SerializeField] private AudioClip m_DestroyClip;               // The audio clip to play when the target shatters.
        [SerializeField] private Boolean m_HasVoronoi;

        private Transform m_CameraTransform;     
                               
        private InteractiveItem m_InteractiveItem;                   
        private AudioSource m_Audio;                                    
        private Renderer m_Renderer;                                   
        private Collider m_Collider;                                    
        private bool m_IsEnding;

        public PKFxFX m_ParticleExplosion;


        private void Awake()
        {
            m_CameraTransform = Camera.main.transform;
            m_Audio = GetComponent<AudioSource> ();
            m_InteractiveItem = GetComponent<InteractiveItem>();
            m_Renderer = GetComponent<Renderer>();
            m_Collider = GetComponent<Collider>();
        }

        private void OnEnable ()
        {
            m_InteractiveItem.OnDown += HandleDown;
        }
        private void OnDisable ()
        {
            m_InteractiveItem.OnDown -= HandleDown;
        }

        private void OnDestroy()
        {
            OnRemove = null;
        }

        void OnTriggerEnter(Collider other)
        {
          
        }

        private void HandleDown()
        {
            if (m_IsEnding)
                return;

            m_IsEnding = true;

            m_Renderer.enabled = false;
            m_Collider.enabled = false;

            m_Audio.clip = m_DestroyClip;
            m_Audio.Play();

            m_ParticleExplosion.StartEffect();

            SessionData.AddScore(m_Score);

            if (m_HasVoronoi)
            {
                GameObject destroyedTarget = Instantiate(m_DestroyPrefab, transform.position, transform.rotation) as GameObject;
                Destroy(destroyedTarget, m_DestroyTimeOutDuration);
            }

            if (OnRemove != null)
                OnRemove(this);
        }
    }
}