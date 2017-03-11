using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace RailShooter.Utils
{
    public class CameraFade : MonoBehaviour
    {
        public event Action OnFadeComplete;                             

        [SerializeField] private Image m_FadeImage;                     
        [SerializeField] private AudioMixerSnapshot m_DefaultSnapshot;  
        [SerializeField] private AudioMixerSnapshot m_FadedSnapshot;    
        [SerializeField] private Color m_FadeColor = Color.black;       
        [SerializeField] private float m_FadeDuration = 2.0f;          
        [SerializeField] private bool m_FadeInOnSceneLoad = false;      
        [SerializeField] private bool m_FadeInOnStart = false;         

        private bool m_IsFading;
        private float m_FadeStartTime;                                            
        private Color m_FadeOutColor;                                  

        public bool IsFading { get { return m_IsFading; } }

        private void Awake()
        {
			SceneManager.sceneLoaded += HandleSceneLoaded;

            m_FadeOutColor = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0f);
            m_FadeImage.enabled = true;
        }

        private void Start()
        {
            if (m_FadeInOnStart)
            {
                m_FadeImage.color = m_FadeColor;
                FadeIn(true);
            }
        }

		private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (m_FadeInOnSceneLoad)
            {
                m_FadeImage.color = m_FadeColor;
                FadeIn(true);
            }
        }

                public void FadeOut(bool fadeAudio)
        {
            FadeOut(m_FadeDuration, fadeAudio);
        }

        public void FadeOut(float duration, bool fadeAudio)
        {
            if (m_IsFading)
                return;
            StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, duration));
            
            if(m_FadedSnapshot && fadeAudio)
                m_FadedSnapshot.TransitionTo (duration);
        }

        public void FadeIn(bool fadeAudio)
        {
            FadeIn(m_FadeDuration, fadeAudio);
        }

        public void FadeIn(float duration, bool fadeAudio)
        {
            if (m_IsFading)
                return;
            StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, duration));

            if(m_DefaultSnapshot && fadeAudio)
                m_DefaultSnapshot.TransitionTo (duration);
        }

        public IEnumerator BeginFadeOut (bool fadeAudio)
        {
            if(m_FadedSnapshot && fadeAudio)
                m_FadedSnapshot.TransitionTo (m_FadeDuration);

            yield return StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, m_FadeDuration));
        }

        public IEnumerator BeginFadeOut(float duration, bool fadeAudio)
        {
            if(m_FadedSnapshot && fadeAudio)
                m_FadedSnapshot.TransitionTo (duration);

            yield return StartCoroutine(BeginFade(m_FadeOutColor, m_FadeColor, duration));
        }

        public IEnumerator BeginFadeIn (bool fadeAudio)
        {
            if(m_DefaultSnapshot && fadeAudio)
                m_DefaultSnapshot.TransitionTo (m_FadeDuration);

            yield return StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, m_FadeDuration));
        }

        public IEnumerator BeginFadeIn(float duration, bool fadeAudio)
        {
            if(m_DefaultSnapshot && fadeAudio)
                m_DefaultSnapshot.TransitionTo (duration);

            yield return StartCoroutine(BeginFade(m_FadeColor, m_FadeOutColor, duration));
        }

        private IEnumerator BeginFade(Color startCol, Color endCol, float duration)
        {
            m_IsFading = true;

            float timer = 0f;
            while (timer <= duration)
            {
                m_FadeImage.color = Color.Lerp(startCol, endCol, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            m_IsFading = false;
            if (OnFadeComplete != null)
                OnFadeComplete();
        }

		void OnDestroy()
		{
			SceneManager.sceneLoaded -= HandleSceneLoaded;
		}
    }
}