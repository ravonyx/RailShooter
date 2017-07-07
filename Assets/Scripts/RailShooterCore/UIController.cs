using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RailShooter.Utils;

namespace RailShooter.Assets
{
    // This simple class encapsulates the UI for
    // the shooter scenes so that the game
    // controller need only reference one thing to
    // control the UI during the games.
    public class UIController : MonoBehaviour
    {
        [SerializeField] private UIFader m_introUI;    
        [SerializeField] private UIFader m_otherEnemiesUI;    
        [SerializeField] private UIFader m_enemiesUI;
        [SerializeField] private UIFader m_healthUI;     
        [SerializeField] private UIFader m_outroUI;     
        [SerializeField] private UIFader m_gameOverUI;
        [SerializeField] private UIFader m_playerUI;    
        [SerializeField] private Text m_totalScore;    
        [SerializeField] private Text m_highScore;      

        public IEnumerator ShowIntroUI()
        {
            yield return StartCoroutine(m_introUI.InteruptAndFadeIn());
        }
        public IEnumerator HideIntroUI()
        {
            yield return StartCoroutine(m_introUI.InteruptAndFadeOut());
        }

        public IEnumerator ShowEnemiesUI()
        {
            yield return StartCoroutine(m_enemiesUI.InteruptAndFadeIn());
        }
        public IEnumerator HideEnemiesUI()
        {
            yield return StartCoroutine(m_enemiesUI.InteruptAndFadeOut());
        }

        public IEnumerator ShowHealthUI()
        {
            yield return StartCoroutine(m_healthUI.InteruptAndFadeIn());
        }
        public IEnumerator HideHealthUI()
        {
            yield return StartCoroutine(m_healthUI.InteruptAndFadeOut());
        }
        public IEnumerator ShowOutroUI()
        {
            m_totalScore.text = SessionData.Score.ToString();
            m_highScore.text = SessionData.HighScore.ToString();

            yield return StartCoroutine(m_outroUI.InteruptAndFadeIn());
        }
        public IEnumerator HideOutroUI()
        {
            yield return StartCoroutine(m_outroUI.InteruptAndFadeOut());
        }

        public IEnumerator ShowGameOverUI()
        {
            yield return StartCoroutine(m_gameOverUI.InteruptAndFadeIn());
        }
        public IEnumerator HideGameOverUI()
        {
            yield return StartCoroutine(m_gameOverUI.InteruptAndFadeOut());
        }

        public IEnumerator ShowPlayerUI ()
        {
            yield return StartCoroutine (m_playerUI.InteruptAndFadeIn ());
        }
        public IEnumerator HidePlayerUI ()
        {
            yield return StartCoroutine (m_playerUI.InteruptAndFadeOut ());
        }

        public IEnumerator ShowOtherEnemiesUI()
        {
            yield return StartCoroutine(m_otherEnemiesUI.InteruptAndFadeIn());
        }
        public IEnumerator HideOtherEnemiesUI()
        {
            yield return StartCoroutine(m_otherEnemiesUI.InteruptAndFadeOut());
        }
    }
}