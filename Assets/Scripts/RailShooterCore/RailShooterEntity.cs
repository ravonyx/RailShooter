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
        private CamerasAndInputsManager m_camInputManager;

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
        private BoxCollider m_collider;

        [SerializeField]
        private PKFxFX m_particleExplosion;
        [SerializeField]
        private PKFxFX m_particleScore;

        [SerializeField]
        private RailShooterController m_shootingGalleryController;

        private void Awake()
        {
            m_light = GetComponent<Light>();
            m_audio = GetComponent<AudioSource>();
            m_interactiveItem = GetComponent<InteractiveItem>();
            m_renderer = GetComponent<Renderer>();
            m_collider = GetComponent<BoxCollider>();
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
            if (m_collider)
                m_collider.enabled = false;

            if (m_audio)
                m_audio.Play();
			if (m_particleExplosion) 
			{
				m_particleExplosion.transform.parent = null;
				m_particleExplosion.StartEffect ();
			}
            if (m_particleScore)
            {
                Vector3 parentPos = m_particleScore.transform.parent.position;
                if (m_gainPoints)
                    m_particleScore.transform.position = new Vector3(parentPos.x, parentPos.y + 12, parentPos.z);
                else
                    m_particleScore.transform.position = new Vector3(parentPos.x, parentPos.y, parentPos.z);

                m_particleScore.transform.parent = null;
                m_particleScore.transform.LookAt(m_camInputManager.CurrentCamera.transform);
                m_particleScore.transform.Rotate(0, 180, 0);

                if(m_gainPoints)
                {
                    PKFxManager.Sampler textAttr = m_particleScore.GetSampler("Text");
                    int score = m_score * SessionData.Multiplicateur;
                    textAttr.m_Text = score.ToString();
                }
                m_particleScore.StartEffect();
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