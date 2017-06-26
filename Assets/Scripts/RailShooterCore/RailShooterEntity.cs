using System;
using System.Collections;
using UnityEngine;
using VRStandardAssets.Utils;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    public class RailShooterEntity : MonoBehaviour
    {
        public event Action<RailShooterEntity> OnRemove;

        [SerializeField]
        private bool m_gainPoints;

        [SerializeField]
        private int m_score = 1;                      

        [SerializeField]
        private float m_destroyTimeOutDuration = 2f;   // When the target is hit, it shatters.  This is how long before the shattered pieces disappear.
        [SerializeField]
        private GameObject m_destroyPrefab;            // The prefab for the shattered target.

        [SerializeField]
        private Boolean m_hasVoronoi;

        private InteractiveItem m_interactiveItem;                   
        private AudioSource m_audio;                                    
        private Light m_light;
        private Renderer m_renderer;                                   
        private bool m_isEnding;

        [SerializeField]
        private PKFxFX m_particleExplosion;
        [SerializeField]
        private RailShooterController m_shootingGalleryController;

        private void Awake()
        {
            m_light = GetComponent<Light>();
            m_audio = GetComponent<AudioSource>();
            m_interactiveItem = GetComponent<InteractiveItem>();
            m_renderer = GetComponent<Renderer>();
            if (m_renderer == null)
                m_renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void OnEnable ()
        {
            m_interactiveItem.OnDown += HandleDown;
            m_interactiveItem.OnDownLeft += HandleDown;
        }
        private void OnDisable ()
        {
            m_interactiveItem.OnDown -= HandleDown;
            m_interactiveItem.OnDownLeft -= HandleDown;
        }

        private void OnDestroy()
        {
            OnRemove = null;
        }

        void OnTriggerEnter(Collider other)
        {
            m_particleExplosion.StartEffect();
            m_renderer.enabled = false;
        }

        private void HandleDown()
        {
            if (m_isEnding || !m_shootingGalleryController.IsPlaying)
                return;

            m_isEnding = true;
            m_renderer.enabled = false;

            if(m_audio)
                m_audio.Play();
			if (m_particleExplosion) 
			{
				m_particleExplosion.transform.parent = null;
				m_particleExplosion.StartEffect ();
			}
            if (m_light)
                m_light.enabled = false;
            if (m_gainPoints)
                SessionData.AddScore(m_score);

            if (m_hasVoronoi)
            {
                GameObject destroyedTarget = Instantiate(m_destroyPrefab, transform.position, transform.rotation) as GameObject;
                Destroy(destroyedTarget, m_destroyTimeOutDuration);
            }
            if (OnRemove != null)
                OnRemove(this);
        }
    }
}