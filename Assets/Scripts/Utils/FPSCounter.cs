using UnityEngine;
using UnityEngine.UI;

namespace RailShooter.Utils
{
    // This script should be placed on a gameobject with a Text component.
    public class FPSCounter : MonoBehaviour
    {
        private float m_DeltaTime;                      
        private Text m_Text;                            
        private const float k_SmoothingCoef = 0.1f;     

        private void Start ()
        {
            m_Text = GetComponent<Text> ();
        }

        private void Update ()
        {
            m_DeltaTime += (Time.deltaTime - m_DeltaTime) * k_SmoothingCoef;
            float fps = 1.0f / m_DeltaTime;
            m_Text.text = Mathf.FloorToInt (fps) + " fps";
            if (Input.GetKeyDown (KeyCode.F))
            {
                m_Text.enabled = !m_Text.enabled;
            }
        }
    }
}