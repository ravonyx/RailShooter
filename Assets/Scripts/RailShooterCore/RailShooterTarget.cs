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
        private Renderer m_renderer;                                   
        private bool m_isEnding;

        public PKFxFX m_particleExplosion;

        private void Awake()
        {
            m_audio = GetComponent<AudioSource> ();
            m_interactiveItem = GetComponent<InteractiveItem>();
            m_renderer = GetComponent<Renderer>();
        }

        private void OnEnable ()
        {
            m_interactiveItem.OnDown += HandleDown;
        }
        private void OnDisable ()
        {
            m_interactiveItem.OnDown -= HandleDown;
        }

        private void OnDestroy()
        {
            OnRemove = null;
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger enter " + gameObject.name);
        }

        private void HandleDown()
        {
            if (m_isEnding)
                return;

            m_isEnding = true;
            m_renderer.enabled = false;

            if(m_audio)
                m_audio.Play();
            if(m_particleExplosion)
                m_particleExplosion.StartEffect();

            if(m_gainPoints)
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