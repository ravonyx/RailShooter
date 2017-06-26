using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace RailShooter.Utils
{
    public class SelectionRadial : MonoBehaviour
    {
        public event Action OnSelectionComplete;                                                

        [SerializeField] private float m_SelectionDuration = 2f;                                
        [SerializeField] private bool m_HideOnStart = true;                                     // Whether or not the bar should be visible at the start.
        [SerializeField] private Image m_Selection;                                             // Reference to the image who's fill amount is adjusted to display the bar.
        [SerializeField]
        private CamerasAndInputsManager m_camInputManager;

        private Inputs m_inputs;
        private Coroutine m_SelectionFillRoutine;                                               // Used to start and stop the filling coroutine based on input.
        private bool m_IsSelectionRadialActive;                                                    // Whether or not the bar is currently useable.
        private bool m_RadialFilled;                                                               // Used to allow the coroutine to wait for the bar to fill.

        public float SelectionDuration { get { return m_SelectionDuration; } }

        private void OnEnable()
        {
            m_inputs = m_camInputManager.CurrentInputs;

            m_inputs.OnDown += HandleDown;
            m_inputs.OnUp += HandleUp;
        }

        private void OnDisable()
        {
            m_inputs.OnDown -= HandleDown;
            m_inputs.OnUp -= HandleUp;
        }

        private void Start()
        {
            m_Selection.fillAmount = 0f;

            if(m_HideOnStart)
                Hide();
        }

        public void Show()
        {
            m_Selection.gameObject.SetActive(true);
            m_IsSelectionRadialActive = true;
        }

        public void Hide()
        {
            m_Selection.gameObject.SetActive(false);
            m_IsSelectionRadialActive = false;

            m_Selection.fillAmount = 0f;            
        }

        private IEnumerator FillSelectionRadial()
        {
            m_RadialFilled = false;

            float timer = 0f;
            m_Selection.fillAmount = 0f;
            
            while (timer < m_SelectionDuration)
            {
                m_Selection.fillAmount = timer / m_SelectionDuration;
                timer += Time.deltaTime;
                yield return null;
            }

            m_Selection.fillAmount = 1f;
            m_IsSelectionRadialActive = false;
            m_RadialFilled = true;

            if (OnSelectionComplete != null)
                OnSelectionComplete();
        }


        public IEnumerator WaitForSelectionRadialToFill ()
        {
            m_RadialFilled = false;
            Show ();
            while (!m_RadialFilled)
            {
                yield return null;
            }
            Hide ();
        }

        private void HandleDown()
        {
            if (m_IsSelectionRadialActive)
                m_SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
        }

        private void HandleUp()
        {
            if (m_IsSelectionRadialActive)
            {
                if(m_SelectionFillRoutine != null)
                    StopCoroutine(m_SelectionFillRoutine);

                m_Selection.fillAmount = 0f;
            }
        }
    }
}