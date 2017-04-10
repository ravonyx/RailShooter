using UnityEngine;
using System.Collections;
using RailShooter.Utils;
using VRStandardAssets.Utils;

namespace RailShooter.Intro
{
    public class IntroManager : MonoBehaviour
    {
        [SerializeField] private Reticle m_Reticle;                        
        [SerializeField] private SelectionRadial m_Radial;                 

        [SerializeField] private UIFader m_HowToUseFader;                   
        [SerializeField] private SelectionSlider m_HowToUseSlider;         
        [SerializeField] private UIFader m_HowToUseConfirmFader;            
        [SerializeField] private SelectionSlider m_HowToUseConfirmSlider;  
        [SerializeField] private UIFader m_ReturnFader;                    

        private IEnumerator Start ()
        {
            m_Reticle.Show ();
            m_Radial.Hide ();

            // In order, fade in the UI on how to use sliders, wait for the slider to be filled then fade out the UI.
            yield return StartCoroutine (m_HowToUseFader.InteruptAndFadeIn ());
            yield return StartCoroutine (m_HowToUseSlider.WaitForBarToFill ());
            yield return StartCoroutine (m_HowToUseFader.InteruptAndFadeOut ());

            // In order, fade in the UI on confirming the use of sliders, wait for the slider to be filled, then fade out the UI.
            yield return StartCoroutine(m_HowToUseConfirmFader.InteruptAndFadeIn());
            yield return StartCoroutine(m_HowToUseConfirmSlider.WaitForBarToFill());
            yield return StartCoroutine(m_HowToUseConfirmFader.InteruptAndFadeOut());

            // Fade in the final UI.
            yield return StartCoroutine (m_ReturnFader.InteruptAndFadeIn ());
        }
    }
}